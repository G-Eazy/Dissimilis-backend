namespace Dissimilis.Core.Collections
{
    public static class ArrayExtensionscs
    {

        public static bool TryGetValue<T>(this T[] array, int position, out T value)
        {
            value = default;

            if (array == null)
            {
                return false;
            }

            if (position > array.Length - 1)
            {
                return false;
            }

            value = array[position];

            if (value == null)
            {
                return false;
            }

            return true;
        }


        public static T GetValueAtPos<T>(this T[] array, int position) =>
            array == null
                ? default
                : position > array.Length - 1
                    ? default
                    : array[position];

    }
}
