namespace OurPlan.Helpers
{
    public class ServiceResult<T>
    {
        public bool IsSuccesful { get; set; }
        public List<string> ValidationMessage { get; set; } = new();
        public T Result { get; set; }
    }
}
