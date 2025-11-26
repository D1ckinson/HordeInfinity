namespace Assets.Code.Tools
{
    public class FPSCalculator
    {
        private const float One = 1f;
        private const string WholeNumberFormat = "0";

        public string Calculate(float deltaTime)
        {
            return (One / deltaTime).ToString(WholeNumberFormat);
        }
    }
}
