using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigaLibre.Domain.Enums
{
    public enum MatchStatusEnum
    {
        Scheduled = 0,
        Inprogress = 1,
        Finished = 2,
        Postponed = 3,
        Canceled = 4
    }
}
