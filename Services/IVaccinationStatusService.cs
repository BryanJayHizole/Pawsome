using System.Collections.Generic;
using Pawsome.Models;

namespace Pawsome.Services
{
    public interface IVaccinationStatusService
    {
        IEnumerable<VaccinationStatusPvet> GetVaccinationStatusData();

    }
}
