import os
import re

# Mapare tipuri C# → TypeScript
TYPE_MAP = {
    'int': 'number',
    'float': 'number',
    'double': 'number',
    'decimal': 'number',
    'string': 'string',
    'bool': 'boolean',
    'DateTime': 'Date',
    'Guid': 'string',
    'void': 'void'
}

def map_type(csharp_type):
    # Elimină genericele și array-urile
    csharp_type = csharp_type.strip()
    csharp_type = re.sub(r'List<(\w+)>', r'\1[]', csharp_type)
    csharp_type = csharp_type.replace("?", "")  # elimină nullable
    return TYPE_MAP.get(csharp_type, csharp_type)

def to_camel_case(name):
    """Convertește numele de la PascalCase la camelCase (prima literă mică)"""
    if not name:
        return name
    return name[0].lower() + name[1:] if len(name) > 1 else name.lower()

def extract_class_info(content):
    class_match = re.search(r'public\s+class\s+(\w+)', content)
    if not class_match:
        return None, []

    class_name = class_match.group(1)

    # Extrage proprietăți publice: ex. public string Name { get; set; }
    properties = re.findall(r'public\s+([\w<>\[\]]+)\s+(\w+)\s*{\s*get;\s*set;\s*}', content)

    return class_name, properties

def convert_to_ts_interface(class_name, properties):
    interface_name = f'I{class_name}'
    lines = [f'export interface {interface_name} {{']
    for csharp_type, name in properties:
        ts_type = map_type(csharp_type)
        camel_name = to_camel_case(name)
        lines.append(f'  {camel_name}: {ts_type};')
    lines.append('}\n')
    return '\n'.join(lines)

def process_folder_to_ts(source_folder, destination_file):
    all_interfaces = []

    for filename in os.listdir(source_folder):
        if filename.endswith('.cs'):
            with open(os.path.join(source_folder, filename), 'r', encoding='utf-8') as f:
                content = f.read()

            class_name, properties = extract_class_info(content)
            if class_name and properties:
                ts_interface = convert_to_ts_interface(class_name, properties)
                all_interfaces.append(ts_interface)
                print(f'✔ Procesat: {filename} → {class_name}')

    with open(destination_file, 'w', encoding='utf-8') as f:
        f.write('\n'.join(all_interfaces))
    print(f'\n✅ Fișierul final a fost creat: {destination_file}')

# Exemplu de utilizare
# Calculează calea relativă bazată pe locația scriptului
script_dir = os.path.dirname(os.path.abspath(__file__))
project_root = os.path.join(script_dir, '../../../../OurPlan')
source_path = os.path.join(project_root, 'OurPlan', 'DTO')
destination_ts_file = os.path.join(script_dir, '..', 'interfaces.ts')

# Convertim la cale absolută pentru a evita problemele cu calea relativă
source_path = os.path.abspath(source_path)
destination_ts_file = os.path.abspath(destination_ts_file)

print(f'📁 Căutând fișiere .cs în: {source_path}')
print(f'💾 Fișierul de ieșire va fi: {destination_ts_file}\n')

process_folder_to_ts(source_path, destination_ts_file)
