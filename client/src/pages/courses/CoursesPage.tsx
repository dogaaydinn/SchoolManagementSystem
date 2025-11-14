import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Plus, Search } from 'lucide-react';

const mockCourses = [
  { id: 1, courseCode: 'CS101', title: 'Introduction to Computer Science', credits: 3, maxStudents: 30, currentEnrollment: 25, isActive: true },
  { id: 2, courseCode: 'MATH201', title: 'Calculus I', credits: 4, maxStudents: 35, currentEnrollment: 32, isActive: true },
  { id: 3, courseCode: 'PHYS101', title: 'Physics I', credits: 4, maxStudents: 25, currentEnrollment: 20, isActive: true },
];

export default function CoursesPage() {
  const [searchTerm, setSearchTerm] = useState('');

  const filteredCourses = mockCourses.filter(
    (course) =>
      course.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      course.courseCode.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Courses</h1>
          <p className="text-gray-500 mt-1">Manage courses and curriculum</p>
        </div>
        <button className="btn btn-primary btn-md">
          <Plus className="h-4 w-4 mr-2" />
          Add Course
        </button>
      </div>

      <div className="card">
        <div className="card-content p-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search courses..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="input pl-10 w-full"
            />
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {filteredCourses.map((course) => (
          <div key={course.id} className="card hover:shadow-lg transition-shadow">
            <div className="card-header">
              <div className="flex items-start justify-between">
                <div>
                  <p className="text-sm font-medium text-primary-600">{course.courseCode}</p>
                  <h3 className="text-lg font-semibold text-gray-900 mt-1">{course.title}</h3>
                </div>
                <span className="badge badge-default">{course.credits} Credits</span>
              </div>
            </div>
            <div className="card-content">
              <div className="space-y-2">
                <div className="flex justify-between text-sm">
                  <span className="text-gray-500">Enrollment</span>
                  <span className="font-medium">{course.currentEnrollment} / {course.maxStudents}</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div
                    className="bg-primary-600 h-2 rounded-full"
                    style={{ width: `${(course.currentEnrollment / course.maxStudents) * 100}%` }}
                  />
                </div>
              </div>
            </div>
            <div className="card-footer">
              <Link to={`/courses/${course.id}`} className="text-sm text-primary-600 hover:text-primary-700 font-medium">
                View Details →
              </Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
