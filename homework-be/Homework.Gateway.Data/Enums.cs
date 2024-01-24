using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework.Gateway.Data
{
    public enum RequestMethodTypeEnum
    {
        Create,
        Update,
        Delete
    };

    public enum QueueRequestStatus
    {
        Done,
        Pending,
        Error
    };
}
