using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Location;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace YACTR.DI.Repository;

public class SectorRepository : EntityRepository<Sector>, ISectorRepository
{
    public SectorRepository(DatabaseContext context, IClock clock)
        : base(context, clock)
    {
    }

    public override async Task<Sector?> GetByIdAsync(Guid id)
    {
        return await _context.Sectors
            .AsNoTracking()
            .Include(s => s.Area)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IEnumerable<Sector>> GetAllAsync()
    {
        return await _context.Sectors
            .AsNoTracking()
            .Include(s => s.Area)
            .ToListAsync();
    }
} 