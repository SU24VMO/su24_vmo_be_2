﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IActivityImageRepository : ICrudBaseRepository<ActivityImage, Guid>
    {
        public IEnumerable<ActivityImage> GetAllActivityImagesByActivityId(Guid activityId);

    }
}
