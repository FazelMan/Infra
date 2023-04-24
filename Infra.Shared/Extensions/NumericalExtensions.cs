namespace Infra.Shared.Extensions
{
    public static class NumericalExtensions
    {
        public static long? SetNullIfZero(this long? input)
        {
            if (input == null || input == 0)
                return null;

            return input;
        }
    }
}