﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IOTapi.Model;

public partial class Entityrole
{
    public int RoleId { get; set; }

    public string Rolename { get; set; }

    public virtual ICollection<Entity> Entities { get; set; } = new List<Entity>();
}