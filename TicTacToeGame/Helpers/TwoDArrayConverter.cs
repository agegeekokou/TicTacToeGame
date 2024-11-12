using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicTacToeGame.Helpers
{
    public class TwoDArrayConverter : JsonConverter<char[,]>
    {
        public override char[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = JsonSerializer.Deserialize<List<List<char>>>(ref reader, options);
            var array = new char[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    array[i, j] = list[i][j];
            return array;
        }

        public override void Write(Utf8JsonWriter writer, char[,] value, JsonSerializerOptions options)
        {
            var list = new List<List<char>>();
            for (int i = 0; i < 3; i++)
            {
                var row = new List<char>();
                for (int j = 0; j < 3; j++)
                    row.Add(value[i, j]);
                list.Add(row);
            }
            JsonSerializer.Serialize(writer, list, options);
        }
    }
}
