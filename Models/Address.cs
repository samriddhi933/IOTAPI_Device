﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace IOTapi.Model;

public partial class Address
{
    public int Id { get; set; }

    public string Addressline1 { get; set; }

    public string Addressline2 { get; set; }

    public string Pincode { get; set; }

    public int? CityId { get; set; }

    public int? Stateid { get; set; }

    public int? Wardid { get; set; }

    public int? Regionid { get; set; }

    public int? Zoneid { get; set; }

    public string Latitude { get; set; }

    public string Longitude { get; set; }

    public int? CountryId { get; set; }

    public string Remarks { get; set; }

    public virtual City City { get; set; }

    public virtual Country Country { get; set; }

    public virtual Region Region { get; set; }

    public virtual State State { get; set; }

    public virtual Ward Ward { get; set; }
}