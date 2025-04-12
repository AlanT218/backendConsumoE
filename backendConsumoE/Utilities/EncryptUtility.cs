using System.Text;

namespace backendConsumoE.Utilities
{
    public class EncryptUtility
    {
        // Matriz de caracteres permitidos
        private static readonly char[] charSet = ".,_*abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

        // Diccionarios para el mapeo fijo y consistente
        private static readonly Dictionary<char, string> charToFixedMap = new Dictionary<char, string>();
        private static readonly Dictionary<string, char> fixedToCharMap = new Dictionary<string, char>();

        // Matriz fija de valores aleatorios de 4 dígitos (no secuencial)
        private static readonly string[] fixedValues = {
        "1067", "1025", "1013", "1042", "1030", "1058", "1021", "1035", "1054", "1009",
        "1020", "1052", "1003", "1046", "1062", "1010", "1037", "1000", "1060", "1015",
        "1004", "1069", "1043", "1008", "1048", "1056", "1016", "1033", "1051", "1019",
        "1027", "1063", "1005", "1050", "1029", "1007", "1065", "1026", "1053", "1017",
        "1036", "1066", "1032", "1001", "1044", "1055", "1023", "1049", "1011", "1022",
        "1012", "1041", "1040", "1068", "1006", "1034", "1047", "1028", "1014", "1057",
        "1038", "1018", "1039", "1024", "1064", "1045", "1002", "1059", "1031", "1043"
    };

        // Inicializa el mapeo fijo con el arreglo de valores desordenado
        static EncryptUtility()
        {
            for (int i = 0; i < charSet.Length; i++)
            {
                charToFixedMap[charSet[i]] = fixedValues[i];
                fixedToCharMap[fixedValues[i]] = charSet[i];
            }
        }

        // Método de cifrado
        public static string EncryptPassword(string password)
        {
            StringBuilder encrypted = new StringBuilder();

            foreach (char c in password)
            {
                if (charToFixedMap.TryGetValue(c, out string fixedValue))
                {
                    encrypted.Append(fixedValue);
                }
                else
                {
                    throw new ArgumentException($"El carácter '{c}' no está permitido en la contraseña.");
                }
            }

            return encrypted.ToString();
        }

        // Método de desencriptado
        public static string DecryptPassword(string encryptedPassword)
        {
            // Verifica que la longitud sea múltiplo de 4
            if (encryptedPassword.Length % 4 != 0)
            {
                throw new ArgumentException("La longitud de la contraseña encriptada es incorrecta.");
            }

            StringBuilder decrypted = new StringBuilder();

            for (int i = 0; i < encryptedPassword.Length; i += 4)
            {
                string part = encryptedPassword.Substring(i, 4);

                if (fixedToCharMap.TryGetValue(part, out char originalChar))
                {
                    decrypted.Append(originalChar);
                }
                else
                {
                    throw new ArgumentException($"El valor encriptado '{part}' no es válido.");
                }
            }

            return decrypted.ToString();
        }
    }
}
//        public static string GetSHA256(string str)
//        {
//            SHA256 sha256 = SHA256Managed.Create();
//            ASCIIEncoding encoding = new ASCIIEncoding();
//            byte[] stream = null;
//            StringBuilder sb = new StringBuilder();
//            stream = sha256.ComputeHash(encoding.GetBytes(str));
//            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
//            return sb.ToString();
//        }
//    }
//}
