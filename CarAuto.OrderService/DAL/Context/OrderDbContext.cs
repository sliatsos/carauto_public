﻿using CarAuto.ClaimParser;
using CarAuto.EFCore.BaseEntity;
using CarAuto.OrderService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarAuto.OrderService.DAL.Context;

public class OrderDbContext : CustomDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options, IClaimParser claimParser) : base(options, claimParser)
    {
    }

    public DbSet<Order> Orders { get; set; }
}
