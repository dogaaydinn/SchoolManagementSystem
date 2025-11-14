import { useParams, Link } from 'react-router-dom';
import { ArrowLeft } from 'lucide-react';

export default function CourseDetailsPage() {
  const { id } = useParams();

  const course = {
    id: Number(id),
    courseCode: 'CS101',
    title: 'Introduction to Computer Science',
    description: 'An introduction to the fundamentals of computer science and programming.',
    credits: 3,
    maxStudents: 30,
    currentEnrollment: 25,
    isActive: true,
  };

  return (
    <div className="space-y-6">
      <div>
        <Link to="/courses" className="inline-flex items-center text-sm text-gray-500 hover:text-gray-700 mb-4">
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Courses
        </Link>
        <h1 className="text-2xl font-bold text-gray-900">{course.title}</h1>
        <p className="text-gray-500 mt-1">{course.courseCode}</p>
      </div>

      <div className="card">
        <div className="card-header">
          <h3 className="card-title">Course Information</h3>
        </div>
        <div className="card-content">
          <dl className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <dt className="text-sm font-medium text-gray-500">Course Code</dt>
              <dd className="mt-1 text-sm text-gray-900">{course.courseCode}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Credits</dt>
              <dd className="mt-1 text-sm text-gray-900">{course.credits}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Max Students</dt>
              <dd className="mt-1 text-sm text-gray-900">{course.maxStudents}</dd>
            </div>
            <div>
              <dt className="text-sm font-medium text-gray-500">Current Enrollment</dt>
              <dd className="mt-1 text-sm text-gray-900">{course.currentEnrollment}</dd>
            </div>
            <div className="sm:col-span-2">
              <dt className="text-sm font-medium text-gray-500">Description</dt>
              <dd className="mt-1 text-sm text-gray-900">{course.description}</dd>
            </div>
          </dl>
        </div>
      </div>
    </div>
  );
}
