using System;
using System.Globalization;
using System.Linq;

namespace Framework
{
    public static class StringExtensions
    {
        public static string SnakeToUpperCamel( this string self )
        {
            if (string.IsNullOrEmpty(self))
            {
                return self;
            }

            return self
                    .Split( new[] { '_' }, StringSplitOptions.RemoveEmptyEntries )
                    .Select( s => char.ToUpperInvariant( s[ 0 ] ) + s.Substring( 1, s.Length - 1 ) )
                    .Aggregate( string.Empty, ( s1, s2 ) => s1 + s2 );
        }
        
        public static string SnakeToLowerCamel( this string self )
        {
            if (string.IsNullOrEmpty(self))
            {
                return self;
            }

            return self
                    .SnakeToUpperCamel()
                    .Insert( 0, char.ToLowerInvariant( self[ 0 ] ).ToString() ).Remove( 1, 1 )
                ;
        }
        
        public static string ToTitleUpperCase( this string self )
        {
            if (string.IsNullOrEmpty(self))
            {
                return self;
            }

            return char.ToUpper(self[0]) + self.Substring(1);
        }
    }
}