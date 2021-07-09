using ChatroomAPI.Model.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatroomAPI.Database
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }

        public DbSet<MessageDto> messages { get; set; }
        public DbSet<MessageTypeDto> messageTypes { get; set; }
        public DbSet<RoomDto> rooms { get; set; }
        public DbSet<UserDto> users { get; set; }
        public DbSet<ParticipantDto> participants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MessageDto>().ToTable("Message");
            modelBuilder.Entity<MessageTypeDto>().ToTable("MessageType");
            modelBuilder.Entity<RoomDto>().ToTable("Room");
            modelBuilder.Entity<UserDto>().ToTable("User");
            modelBuilder.Entity<ParticipantDto>().ToTable("Participant");
        }
    }
}
