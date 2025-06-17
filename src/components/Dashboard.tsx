import React from 'react';
import { 
  FileText, 
  TrendingUp, 
  Clock, 
  CheckCircle,
  BarChart3,
  PieChart,
  Activity
} from 'lucide-react';

export const Dashboard: React.FC = () => {
  const stats = [
    {
      title: 'Documents Processed',
      value: '2,847',
      change: '+12%',
      icon: FileText,
      color: 'from-blue-500 to-blue-600'
    },
    {
      title: 'Classification Accuracy',
      value: '94.2%',
      change: '+2.1%',
      icon: TrendingUp,
      color: 'from-green-500 to-green-600'
    },
    {
      title: 'Avg Processing Time',
      value: '1.3s',
      change: '-0.2s',
      icon: Clock,
      color: 'from-purple-500 to-purple-600'
    },
    {
      title: 'Successfully Classified',
      value: '2,682',
      change: '+15%',
      icon: CheckCircle,
      color: 'from-amber-500 to-amber-600'
    }
  ];

  const recentDocuments = [
    { name: 'Invoice_Q4_2024.pdf', type: 'Invoice', confidence: 98.5, time: '2 min ago' },
    { name: 'Employment_Agreement.docx', type: 'Contract', confidence: 95.2, time: '5 min ago' },
    { name: 'John_Doe_Resume.pdf', type: 'Resume', confidence: 97.8, time: '12 min ago' },
    { name: 'Purchase_Order_001.pdf', type: 'Purchase Order', confidence: 96.3, time: '18 min ago' },
    { name: 'NDA_Template.docx', type: 'Agreement', confidence: 94.7, time: '25 min ago' }
  ];

  const documentTypes = [
    { type: 'Invoices', count: 1247, percentage: 43.8, color: 'bg-blue-500' },
    { type: 'Resumes', count: 689, percentage: 24.2, color: 'bg-green-500' },
    { type: 'Contracts', count: 456, percentage: 16.0, color: 'bg-purple-500' },
    { type: 'Purchase Orders', count: 285, percentage: 10.0, color: 'bg-amber-500' },
    { type: 'Others', count: 170, percentage: 6.0, color: 'bg-gray-500' }
  ];

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Dashboard</h1>
        <p className="text-gray-600">Monitor your document classification performance and insights</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <div key={index} className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg hover:shadow-xl transition-all duration-300">
              <div className="flex items-center justify-between mb-4">
                <div className={`p-3 rounded-xl bg-gradient-to-r ${stat.color}`}>
                  <Icon className="w-6 h-6 text-white" />
                </div>
                <span className="text-sm font-medium text-green-600">{stat.change}</span>
              </div>
              <h3 className="text-2xl font-bold text-gray-900 mb-1">{stat.value}</h3>
              <p className="text-gray-600 text-sm">{stat.title}</p>
            </div>
          );
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Recent Documents */}
        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-semibold text-gray-900">Recent Classifications</h2>
            <Activity className="w-5 h-5 text-gray-500" />
          </div>
          <div className="space-y-4">
            {recentDocuments.map((doc, index) => (
              <div key={index} className="flex items-center justify-between p-4 bg-white/50 rounded-xl border border-white/30">
                <div className="flex-1">
                  <p className="font-medium text-gray-900 truncate">{doc.name}</p>
                  <div className="flex items-center space-x-4 mt-1">
                    <span className="text-sm text-blue-600 font-medium">{doc.type}</span>
                    <span className="text-sm text-gray-500">{doc.time}</span>
                  </div>
                </div>
                <div className="text-right">
                  <div className="text-sm font-medium text-green-600">{doc.confidence}%</div>
                  <div className="text-xs text-gray-500">confidence</div>
                </div>
              </div>
            ))}
          </div>
        </div>

        {/* Document Types Distribution */}
        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-semibold text-gray-900">Document Types</h2>
            <PieChart className="w-5 h-5 text-gray-500" />
          </div>
          <div className="space-y-4">
            {documentTypes.map((type, index) => (
              <div key={index} className="space-y-2">
                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-gray-700">{type.type}</span>
                  <span className="text-sm text-gray-500">{type.count}</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div 
                    className={`h-2 rounded-full ${type.color}`}
                    style={{ width: `${type.percentage}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};