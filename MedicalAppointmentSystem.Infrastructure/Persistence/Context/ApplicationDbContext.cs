using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointmentSystem.Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

   
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.Patient_Id);
                entity.Property(e => e.Full_Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Contact_Number).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(200);
            });

   
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.Doctor_Id);
                entity.Property(e => e.Full_Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Specialization).HasMaxLength(100);
                entity.Property(e => e.Contact_Number).HasMaxLength(20);
            });

      
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Appointment_Id);
                entity.Property(e => e.Visit_Type).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Diagnosis).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(500);

   
                entity.HasOne(e => e.Patient)
                      .WithMany(p => p.Appointments)
                      .HasForeignKey(e => e.Patient_Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Doctor)
                      .WithMany(d => d.Appointments)
                      .HasForeignKey(e => e.Doctor_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.HasKey(e => e.Medicine_Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
            });


            modelBuilder.Entity<PrescriptionDetail>(entity =>
            {
                entity.HasKey(e => e.Prescription_Id);
                entity.Property(e => e.Dosage).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Notes).HasMaxLength(200);


                entity.HasOne(e => e.Appointment)
                      .WithMany(a => a.PrescriptionDetails)
                      .HasForeignKey(e => e.Appointment_Id)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Medicine)
                      .WithMany(m => m.PrescriptionDetails)
                      .HasForeignKey(e => e.Medicine_Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}