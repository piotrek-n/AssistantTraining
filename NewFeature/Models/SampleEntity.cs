using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewFeature.Models
{
    public class SampleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public SampleEntity() { }
        public SampleEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }



        private static IEnumerable<SampleEntity> SampleData;
        public static IEnumerable<SampleEntity> GetSampleData()
        {
            if (SampleEntity.SampleData == null)
            {
                var _data = new List<SampleEntity>(53);

                var random = new Random();
                for (int i = 0; i < 53; i++)
                {
                    var letter1 = random.Next(65, 91);
                    var letter2 = random.Next(65, 91);
                    var letter3 = random.Next(65, 91);
                    var letter4 = random.Next(65, 91);

                    var sampleEntity = new SampleEntity(i + 1, new string(new[] { (char)letter1, (char)letter2, (char)letter3, (char)letter4 }));
                    _data.Add(sampleEntity);
                }

                SampleEntity.SampleData = _data;
            }

            return SampleEntity.SampleData;
        }
    }
}