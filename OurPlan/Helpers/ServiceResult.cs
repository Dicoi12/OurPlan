namespace OurPlan.Helpers
{
    public class ServiceResult<T>
    {
        private T _result;
        public T? Result
        {
            get => _result;
            set
            {
                _result = value;
                IsSuccesful = _result != null;
            }
        }

        public bool IsSuccesful { get; private set; }
        public List<string> ValidationMessage { get; set; } = new();
    }
}
