import { useParams, Link } from 'react-router-dom';
import { ArrowLeft, Mail, Phone, Calendar, Award } from 'lucide-react';

export default function StudentDetailsPage() {
  const { id } = useParams();

  // Mock data - replace with real API call
  const student = {
    id: Number(id),
    studentNumber: 'STU202300001',
    fullName: 'John Smith',
    email: 'john.smith@school.edu',
    phone: '+1 (555) 123-4567',
    dateOfBirth: '2000-05-15',
    enrollmentDate: '2023-09-01',
    gpa: 3.75,
    isActive: true,
    address: '123 Main St, Anytown, ST 12345',
  };

  return (
    <div className="space-y-6">
      <div>
        <Link to="/students" className="inline-flex items-center text-sm text-gray-500 hover:text-gray-700 mb-4">
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Students
        </Link>
        <h1 className="text-2xl font-bold text-gray-900">{student.fullName}</h1>
        <p className="text-gray-500 mt-1">{student.studentNumber}</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Student Information */}
        <div className="lg:col-span-2 space-y-6">
          <div className="card">
            <div className="card-header">
              <h3 className="card-title">Personal Information</h3>
            </div>
            <div className="card-content">
              <dl className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <dt className="text-sm font-medium text-gray-500">Email</dt>
                  <dd className="mt-1 text-sm text-gray-900 flex items-center">
                    <Mail className="h-4 w-4 mr-2 text-gray-400" />
                    {student.email}
                  </dd>
                </div>
                <div>
                  <dt className="text-sm font-medium text-gray-500">Phone</dt>
                  <dd className="mt-1 text-sm text-gray-900 flex items-center">
                    <Phone className="h-4 w-4 mr-2 text-gray-400" />
                    {student.phone}
                  </dd>
                </div>
                <div>
                  <dt className="text-sm font-medium text-gray-500">Date of Birth</dt>
                  <dd className="mt-1 text-sm text-gray-900 flex items-center">
                    <Calendar className="h-4 w-4 mr-2 text-gray-400" />
                    {new Date(student.dateOfBirth).toLocaleDateString()}
                  </dd>
                </div>
                <div>
                  <dt className="text-sm font-medium text-gray-500">Enrollment Date</dt>
                  <dd className="mt-1 text-sm text-gray-900 flex items-center">
                    <Calendar className="h-4 w-4 mr-2 text-gray-400" />
                    {new Date(student.enrollmentDate).toLocaleDateString()}
                  </dd>
                </div>
                <div className="sm:col-span-2">
                  <dt className="text-sm font-medium text-gray-500">Address</dt>
                  <dd className="mt-1 text-sm text-gray-900">{student.address}</dd>
                </div>
              </dl>
            </div>
          </div>
        </div>

        {/* Quick Stats */}
        <div className="space-y-6">
          <div className="card">
            <div className="card-header">
              <h3 className="card-title">Academic Overview</h3>
            </div>
            <div className="card-content">
              <div className="space-y-4">
                <div>
                  <p className="text-sm font-medium text-gray-500">Current GPA</p>
                  <p className="text-3xl font-bold text-primary-600 mt-1 flex items-center">
                    <Award className="h-6 w-6 mr-2" />
                    {student.gpa.toFixed(2)}
                  </p>
                </div>
                <div className="border-t pt-4">
                  <p className="text-sm font-medium text-gray-500">Status</p>
                  <span className={`badge mt-1 ${student.isActive ? 'badge-default' : 'badge-secondary'}`}>
                    {student.isActive ? 'Active' : 'Inactive'}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
