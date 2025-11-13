import { Users, BookOpen, GraduationCap, TrendingUp } from 'lucide-react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, LineChart, Line } from 'recharts';

// Mock data - in real app, this would come from API
const stats = [
  {
    name: 'Total Students',
    value: '2,543',
    change: '+12%',
    changeType: 'increase',
    icon: Users,
    color: 'bg-blue-500',
  },
  {
    name: 'Active Courses',
    value: '48',
    change: '+3',
    changeType: 'increase',
    icon: BookOpen,
    color: 'bg-green-500',
  },
  {
    name: 'Total Teachers',
    value: '127',
    change: '+5%',
    changeType: 'increase',
    icon: GraduationCap,
    color: 'bg-purple-500',
  },
  {
    name: 'Average GPA',
    value: '3.42',
    change: '+0.05',
    changeType: 'increase',
    icon: TrendingUp,
    color: 'bg-yellow-500',
  },
];

const enrollmentData = [
  { month: 'Jan', students: 2100 },
  { month: 'Feb', students: 2250 },
  { month: 'Mar', students: 2300 },
  { month: 'Apr', students: 2400 },
  { month: 'May', students: 2450 },
  { month: 'Jun', students: 2543 },
];

const gradeDistribution = [
  { grade: 'A', count: 450 },
  { grade: 'B', count: 680 },
  { grade: 'C', count: 520 },
  { grade: 'D', count: 180 },
  { grade: 'F', count: 70 },
];

const recentActivities = [
  {
    id: 1,
    type: 'enrollment',
    message: 'John Smith enrolled in Advanced Mathematics',
    time: '2 hours ago',
  },
  {
    id: 2,
    type: 'grade',
    message: 'New grades posted for Physics 101',
    time: '4 hours ago',
  },
  {
    id: 3,
    type: 'assignment',
    message: 'New assignment created for Computer Science 201',
    time: '6 hours ago',
  },
  {
    id: 4,
    type: 'enrollment',
    message: 'Sarah Johnson enrolled in Biology Lab',
    time: '8 hours ago',
  },
];

export default function DashboardPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-500 mt-1">Welcome back! Here's what's happening today.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat) => {
          const Icon = stat.icon;
          return (
            <div key={stat.name} className="card">
              <div className="card-content p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm font-medium text-gray-600">{stat.name}</p>
                    <p className="text-3xl font-bold text-gray-900 mt-2">{stat.value}</p>
                    <p className={`text-sm mt-2 ${
                      stat.changeType === 'increase' ? 'text-green-600' : 'text-red-600'
                    }`}>
                      {stat.change} from last month
                    </p>
                  </div>
                  <div className={`${stat.color} p-3 rounded-lg`}>
                    <Icon className="h-6 w-6 text-white" />
                  </div>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Enrollment Trend */}
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">Student Enrollment Trend</h3>
            <p className="card-description">Monthly enrollment numbers for the current year</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={enrollmentData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Line type="monotone" dataKey="students" stroke="#3b82f6" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Grade Distribution */}
        <div className="card">
          <div className="card-header">
            <h3 className="card-title">Grade Distribution</h3>
            <p className="card-description">Current semester grade distribution</p>
          </div>
          <div className="card-content">
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={gradeDistribution}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="grade" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="count" fill="#3b82f6" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="card">
        <div className="card-header">
          <h3 className="card-title">Recent Activity</h3>
          <p className="card-description">Latest updates and changes in the system</p>
        </div>
        <div className="card-content">
          <div className="space-y-4">
            {recentActivities.map((activity) => (
              <div key={activity.id} className="flex items-start gap-4 pb-4 border-b last:border-b-0">
                <div className="flex-shrink-0">
                  <div className="h-10 w-10 rounded-full bg-primary-100 flex items-center justify-center">
                    <Users className="h-5 w-5 text-primary-600" />
                  </div>
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-900">{activity.message}</p>
                  <p className="text-sm text-gray-500 mt-1">{activity.time}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
