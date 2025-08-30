using YACTR.Data;

namespace YACTR.Tests.TestData
{
    public class TestDataSeeder
    {
        private readonly DatabaseContext _context;

        public TestDataSeeder(DatabaseContext context)
        {
            _context = context;
        }
    }
}