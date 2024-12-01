using nike_website_backend.Interfaces;
using nike_website_backend.Models;

namespace nike_website_backend.Services
{
    public class BagService: IBagRepository
    {
        private readonly ApplicationDbContext _context;

        public BagService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
