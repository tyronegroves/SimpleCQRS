namespace EventSourcingCQRS
{
    public interface IHaveATestMode
    {
        bool IsInTestMode { get; set; }
    }

    namespace Testing
    {
        public static class TestModeExtensionMethods
        {
            public static void TurnTestModeOn(this IHaveATestMode value)
            {
                value.IsInTestMode = true;
            }

            public static void TurnTestModeOff(this IHaveATestMode value)
            {
                value.IsInTestMode = false;
            }
        }
    }
}