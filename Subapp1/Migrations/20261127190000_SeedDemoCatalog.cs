using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using StudyRoomBooking.Models;

#nullable disable

namespace StudyRoomBooking.Mvc.Migrations;

[DbContext(typeof(StudyRoomDbContext))]
[Migration("20261127190000_SeedDemoCatalog")]
public class SeedDemoCatalog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Fill to eight buildings without changing locations already created by admins.
        migrationBuilder.Sql("""
            WITH candidates(Name, Address, SortOrder) AS (
                VALUES
                    ('Pilestredet 46', 'Pilestredet 46, Oslo', 1),
                    ('Pilestredet 48', 'Pilestredet 48, Oslo', 2),
                    ('Pilestredet 52', 'Pilestredet 52, Oslo', 3),
                    ('Pilestredet 56', 'Pilestredet 56, Oslo', 4),
                    ('St. Olavs gate 32', 'St. Olavs gate 32, Oslo', 5),
                    ('Pilestredet 40', 'Pilestredet 40, Oslo', 6),
                    ('Pilestredet 60', 'Pilestredet 60, Oslo', 7),
                    ('Pilestredet 62', 'Pilestredet 62, Oslo', 8)
            )
            INSERT INTO Buildings (Name, Address)
            SELECT c.Name, c.Address
            FROM candidates c
            WHERE NOT EXISTS (
                SELECT 1 FROM Buildings b WHERE b.Name = c.Name COLLATE NOCASE
            )
            ORDER BY c.SortOrder
            LIMIT (SELECT max(0, 8 - COUNT(*)) FROM Buildings);
            """);

        // The room numbers repeat across buildings, as they do in the existing catalog.
        migrationBuilder.Sql("""
            WITH candidates(BuildingName, RoomNumber, Capacity, SortOrder) AS (
                VALUES
                    ('Pilestredet 46', '101', 4, 1),
                    ('Pilestredet 48', '201', 6, 2),
                    ('Pilestredet 52', '301', 8, 3),
                    ('Pilestredet 56', '401', 10, 4),
                    ('St. Olavs gate 32', '501', 4, 5),
                    ('Pilestredet 40', '601', 6, 6),
                    ('Pilestredet 46', '102', 8, 7),
                    ('Pilestredet 48', '202', 10, 8),
                    ('Pilestredet 52', '302', 12, 9),
                    ('Pilestredet 56', '402', 6, 10),
                    ('St. Olavs gate 32', '502', 8, 11),
                    ('Pilestredet 40', '602', 10, 12),
                    ('Pilestredet 46', '103', 12, 13),
                    ('Pilestredet 48', '203', 4, 14),
                    ('Pilestredet 52', '303', 6, 15),
                    ('Pilestredet 56', '403', 8, 16),
                    ('St. Olavs gate 32', '503', 12, 17),
                    ('Pilestredet 40', '603', 4, 18),
                    ('Pilestredet 46', '104', 6, 19),
                    ('Pilestredet 48', '204', 8, 20),
                    ('Pilestredet 52', '304', 10, 21),
                    ('Pilestredet 56', '404', 12, 22),
                    ('St. Olavs gate 32', '504', 6, 23),
                    ('Pilestredet 40', '604', 8, 24)
            )
            INSERT INTO Rooms (BuildingId, RoomNumber, Capacity)
            SELECT b.Id, c.RoomNumber, c.Capacity
            FROM candidates c
            JOIN Buildings b ON b.Name = c.BuildingName COLLATE NOCASE
            WHERE NOT EXISTS (
                SELECT 1 FROM Rooms r
                WHERE r.BuildingId = b.Id AND r.RoomNumber = c.RoomNumber
            )
            ORDER BY c.SortOrder
            LIMIT (SELECT max(0, 20 - COUNT(*)) FROM Rooms);
            """);

        migrationBuilder.Sql("""
            WITH candidates(CourseCode, Name, Description, SortOrder) AS (
                VALUES
                    ('DATA1000', 'Introduction to Programming', 'Programming fundamentals and problem solving.', 1),
                    ('DATA1100', 'Web Development', 'Building accessible web applications.', 2),
                    ('DATA1200', 'Data Structures', 'Core data structures and their applications.', 3),
                    ('DATA1300', 'Software Engineering', 'Designing and maintaining software systems.', 4),
                    ('DATA1700', 'Human Computer Interaction', 'Designing usable digital experiences.', 5),
                    ('DATA1800', 'Cloud Computing', 'Cloud services and deployment fundamentals.', 6),
                    ('DATA1900', 'Cybersecurity', 'Security principles for modern systems.', 7),
                    ('MAT1000', 'Calculus', 'Functions, limits, and differentiation.', 8),
                    ('MAT1100', 'Linear Algebra', 'Vectors, matrices, and linear systems.', 9),
                    ('MAT1200', 'Statistics', 'Probability and statistical reasoning.', 10),
                    ('ENG1000', 'Academic Writing', 'Writing and presenting academic work.', 11),
                    ('ENG1100', 'Technical Communication', 'Clear communication in technical projects.', 12),
                    ('DES1000', 'Interaction Design', 'Methods for designing interactive products.', 13),
                    ('BUS1000', 'Project Management', 'Planning and managing team projects.', 14),
                    ('BUS1100', 'Entrepreneurship', 'Developing and evaluating new ideas.', 15),
                    ('PHY1000', 'Physics for Computing', 'Physical principles used in technology.', 16),
                    ('BIO1000', 'Introduction to Biology', 'Cells, organisms, and biological systems.', 17),
                    ('HIS1000', 'Modern History', 'Major developments in modern history.', 18),
                    ('SOC1000', 'Research Methods', 'Collecting and evaluating research evidence.', 19),
                    ('ECO1000', 'Microeconomics', 'Markets and economic decision making.', 20),
                    ('CHE1000', 'General Chemistry', 'Chemical principles and laboratory concepts.', 21),
                    ('MED1000', 'Health Informatics', 'Information systems in health services.', 22),
                    ('LAW1000', 'Digital Ethics', 'Ethical and legal issues in technology.', 23),
                    ('GEO1000', 'Urban Studies', 'How cities and communities develop.', 24)
            )
            INSERT INTO Courses (CourseCode, Name, Description)
            SELECT c.CourseCode, c.Name, c.Description
            FROM candidates c
            WHERE NOT EXISTS (
                SELECT 1 FROM Courses existing
                WHERE existing.CourseCode = c.CourseCode COLLATE NOCASE
            )
            ORDER BY c.SortOrder
            LIMIT (SELECT max(0, 20 - COUNT(*)) FROM Courses);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Keep catalog records: bookings may now reference these rooms or courses.
    }
}
