import { useState } from 'react';
import { Plus, Search } from 'lucide-react';

const mockGrades = [
  { id: 1, studentName: 'John Smith', courseName: 'Introduction to CS', value: 95, maxValue: 100, letterGrade: 'A', percentage: 95, gradedAt: '2024-03-15' },
  { id: 2, studentName: 'Sarah Johnson', courseName: 'Calculus I', value: 88, maxValue: 100, letterGrade: 'B+', percentage: 88, gradedAt: '2024-03-14' },
  { id: 3, studentName: 'Michael Brown', courseName: 'Physics I', value: 92, maxValue: 100, letterGrade: 'A-', percentage: 92, gradedAt: '2024-03-13' },
];

export default function GradesPage() {
  const [searchTerm, setSearchTerm] = useState('');

  const filteredGrades = mockGrades.filter(
    (grade) =>
      grade.studentName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      grade.courseName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Grades</h1>
          <p className="text-gray-500 mt-1">Manage student grades and assessments</p>
        </div>
        <button className="btn btn-primary btn-md">
          <Plus className="h-4 w-4 mr-2" />
          Add Grade
        </button>
      </div>

      <div className="card">
        <div className="card-content p-4">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search grades..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="input pl-10 w-full"
            />
          </div>
        </div>
      </div>

      <div className="card">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Student
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Course
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Score
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Percentage
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Letter Grade
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Graded Date
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredGrades.map((grade) => (
                <tr key={grade.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                    {grade.studentName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {grade.courseName}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {grade.value} / {grade.maxValue}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {grade.percentage}%
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className="badge badge-default">{grade.letterGrade}</span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(grade.gradedAt).toLocaleDateString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
