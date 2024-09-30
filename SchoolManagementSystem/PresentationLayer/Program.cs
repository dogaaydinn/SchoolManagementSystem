﻿using SchoolManagementSystem.Data;
using SchoolManagementSystem.Interfaces;
using SchoolManagementSystem.Interfaces.ActionHandler;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Models.Concrete;
using SchoolManagementSystem.PresentationLayer.Handlers;

namespace SchoolManagementSystem.PresentationLayer;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Initialize data with required parameters
        List<Teacher> teachers = DataProvider.GetTeachers(10, "example");
        List<Student> students = DataProvider.GetStudents(20, "example");
        List<Course>? courses = DataProvider.GetCourses(teachers, "example");

        // Display the main menu
        MenuHandler.DisplayMainMenu(students, courses, teachers);
        
        // Demonstrate school actions
        ISchoolActions school = new School(); // Assuming you have a School class that implements ISchoolActions
        Course course = courses[0]; // Example course
        SchoolActionDemonstrator.DemonstrateSchoolActions(school, course);
    }
}