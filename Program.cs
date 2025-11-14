using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace taskschool
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }

        public List<Course> EnrolledCourses { get; set; } = new List<Course>();
    }
    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }

        public List<Course> Courses { get; set; } = new List<Course>();
    }
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }

        public List<Student> EnrolledStudents { get; set; } = new List<Student>();

        public List<Teacher> CourseTeachers { get; set; } = new List<Teacher>();
    }

    public class SchoolData
    {
        private List<Student> _students = new List<Student>();
        private List<Course> _courses = new List<Course>();
        private List<Teacher> _teachers = new List<Teacher>();

        public SchoolData()
        {
            SetupData();
        }
        private void SetupData()
        {
            var t1 = new Teacher { TeacherId = 1, Name = "mr.Ahmed" };
            var t2 = new Teacher { TeacherId = 2, Name = "mrs.Farah" };
            var t3 = new Teacher { TeacherId = 3, Name = "mr.Khalid" };
            _teachers.AddRange(new[] { t1, t2, t3 });

            var c1 = new Course { CourseId = 1001, Title = "Math" };
            var c2 = new Course { CourseId = 1002, Title = "Science" };
            var c3 = new Course { CourseId = 1003, Title = "arabic" };
            _courses.AddRange(new[] { c1, c2, c3 });

            t1.Courses.Add(c1); c1.CourseTeachers.Add(t1);
            t1.Courses.Add(c2); c2.CourseTeachers.Add(t1);
            t2.Courses.Add(c3); c3.CourseTeachers.Add(t2);

            var s1 = new Student { StudentId = 5001, Name = "ahmed" };
            var s2 = new Student { StudentId = 5002, Name = "nazem" };
            var s3 = new Student { StudentId = 5003, Name = "mohamed" };
            var s4 = new Student { StudentId = 5004, Name = "fayed" };
            _students.AddRange(new[] { s1, s2, s3, s4 });

            s1.EnrolledCourses.Add(c1); c1.EnrolledStudents.Add(s1);
            s2.EnrolledCourses.Add(c1); c1.EnrolledStudents.Add(s2);
            s3.EnrolledCourses.Add(c1); c1.EnrolledStudents.Add(s3);

            s1.EnrolledCourses.Add(c2); c2.EnrolledStudents.Add(s1);
            s4.EnrolledCourses.Add(c2); c2.EnrolledStudents.Add(s4);

            s2.EnrolledCourses.Add(c3); c3.EnrolledStudents.Add(s2);
        }
        public Course GetMostEnrolledCourse()
        {
            return _courses.AsParallel()
                           .OrderByDescending(c => c.EnrolledStudents.Count)
                           .FirstOrDefault();
        }
        public IEnumerable<dynamic> GetCourseEnrollmentCounts()
        {
            return _courses.AsParallel()
                           .Select(c => new
                           {
                               Name = c.Title,
                               Count = c.EnrolledStudents.Count
                           });
        }
        public bool DoesTeacherHaveMultipleCourses(int teacherId)
        {
            var teacher = _teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (teacher == null) return false;

            return teacher.Courses.Count > 1;
        }
        public int GetTeacherTotalStudentCount(int teacherId)
        {
            var teacher = _teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (teacher == null) return 0;

            return teacher.Courses
                          .AsParallel()
                          .Sum(c => c.EnrolledStudents.Count);
        }
        public void RunReports()
        {
            Console.WriteLine("        School Reports           ");
            var mostEnrolled = GetMostEnrolledCourse();
            Console.WriteLine($"\n2. Most Enrolled Course :");
            if (mostEnrolled != null)
            {
                Console.WriteLine($"- Course: {mostEnrolled.Title}, Students: {mostEnrolled.EnrolledStudents.Count}");
            }

            Console.WriteLine($"\n3. Enrollment Count Per Course (PLINQ):");
            var enrollmentCounts = GetCourseEnrollmentCounts();
            foreach (dynamic item in enrollmentCounts)
            {
                Console.WriteLine($"- Course: {item.Name}, Students: {item.Count}");
            }

            int teacherIdToCheck = 1;
            bool hasMultiple = DoesTeacherHaveMultipleCourses(teacherIdToCheck);
            Console.WriteLine($"\n4. Teacher ID {teacherIdToCheck} has multiple courses? {hasMultiple}");

            int teacherIdForStudents = 1;
            int totalStudents = GetTeacherTotalStudentCount(teacherIdForStudents);
            Console.WriteLine($"\n5. Total students for Teacher ID {teacherIdForStudents} : {totalStudents}");
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var data = new SchoolData();
            data.RunReports();
        }
    }
}