import React, { useState, useEffect } from 'react';
import {
  GraduationCap,
  Upload,
  Plus,
  X,
  Play,
  BarChart3,
  CheckCircle,
  AlertTriangle,
  TrendingUp
} from 'lucide-react';
import {
  uploadTrainingData,
  getTrainingData,
  validateTrainingData,
  startTraining,
  getTrainingStatus,
  getModelMetrics
} from '../services/trainingService';

interface TrainingData {
  id: string;
  filename: string;
  label: string;
  confidence: number;
  status: 'pending' | 'validated' | 'training';
}

interface ModelMetrics {
  accuracy: number;
  precision: number;
  recall: number;
  f1Score: number;
}

interface TrainingProgress {
  totalDocuments: number;
  processedDocuments: number;
  estimatedTimeSeconds: number | null;
}

export const TrainingModule: React.FC = () => {
  const [trainingData, setTrainingData] = useState<TrainingData[]>([]);
  const [newLabel, setNewLabel] = useState('');
  const [isTraining, setIsTraining] = useState(false);
  const [showAddLabel, setShowAddLabel] = useState(false);
  const [file, setFile] = useState<File | null>(null);
  const [label, setLabel] = useState('Invoice');
  const [jobId, setJobId] = useState<string | null>(null);
  const [metrics, setMetrics] = useState<ModelMetrics | null>(null);
  const [uploading, setUploading] = useState(false);
  const [statusMessage, setStatusMessage] = useState<string | null>(null);
  const [progress, setProgress] = useState<TrainingProgress | null>(null);

  const documentTypes = [
    'Invoice', 'Resume', 'Contract', 'Purchase Order', 'Agreement', 'Report', 'Other'
  ];

  useEffect(() => {
    fetchTrainingData();
    fetchModelMetrics();
    if (jobId) {
      const interval = setInterval(() => {
        getTrainingStatus(jobId).then(res => {
          setStatusMessage(res.data.status);
          setProgress({
            totalDocuments: res.data.totalDocuments,
            processedDocuments: res.data.processedDocuments,
            estimatedTimeSeconds: res.data.estimatedTimeSeconds,
          });
          if (res.data.status === 'Completed' || res.data.status === 'Failed') {
            clearInterval(interval);
            setIsTraining(false);
            setProgress(null);
            fetchModelMetrics();
          }
        });
      }, 2000);
      return () => clearInterval(interval);
    }
  }, [jobId]);

  const fetchTrainingData = async () => {
    try {
      const res = await getTrainingData();
      setTrainingData(
        res.data.map((item: any) => ({
          id: item.id,
          filename: item.fileName || item.filename, // support both
          label: item.label,
          confidence: item.confidence,
          status: (item.status || '').toLowerCase(), // normalize status
        }))
      );
    } catch (err) {
      // handle error
    }
  };

  const fetchModelMetrics = async () => {
    try {
      const res = await getModelMetrics();
      setMetrics(res.data);
    } catch (err) {
      // handle error
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setFile(e.target.files[0]);
    }
  };

  const handleUpload = async () => {
    if (!file || !label) return;
    setUploading(true);
    try {
      await uploadTrainingData(file, label);
      setFile(null);
      fetchTrainingData();
    } catch (err) {
      // handle error
    } finally {
      setUploading(false);
    }
  };

  const handleLabelChange = async (id: string, newLabel: string) => {
    setTrainingData(prev => prev.map(item =>
      item.id === id ? { ...item, label: newLabel, status: 'pending' as const } : item
    ));
    // Optionally, call an API to update label
  };

  const handleValidateData = async (id: string) => {
    try {
      await validateTrainingData(id);
      fetchTrainingData();
    } catch (err) {
      // handle error
    }
  };

  const handleRemoveData = (id: string) => {
    setTrainingData(prev => prev.filter(item => item.id !== id));
    // Optionally, call an API to delete data
  };

  const handleStartTraining = async () => {
    setIsTraining(true);
    try {
      const res = await startTraining();
      setJobId(res.data.jobId);
      setStatusMessage('Training started...');
    } catch (err) {
      setIsTraining(false);
      // handle error
    }
  };

  const getConfidenceColor = (confidence: number) => {
    if (confidence >= 95) return 'text-green-600';
    if (confidence >= 85) return 'text-yellow-600';
    return 'text-red-600';
  };

  const addNewLabel = () => {
    if (newLabel.trim() && !documentTypes.includes(newLabel)) {
      documentTypes.push(newLabel);
      setNewLabel('');
      setShowAddLabel(false);
    }
  };

  const getStatusIcon = (status: TrainingData['status']) => {
    switch (status) {
      case 'pending':
        return <AlertTriangle className="w-5 h-5 text-amber-500" />;
      case 'validated':
        return <CheckCircle className="w-5 h-5 text-green-500" />;
      case 'training':
        return <div className="w-5 h-5 border-2 border-blue-500 border-t-transparent rounded-full animate-spin" />;
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Model Training</h1>
        <p className="text-gray-600">Improve classification accuracy by training with labeled data</p>
      </div>

      {/* Model Performance Metrics */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-4">
            <BarChart3 className="w-8 h-8 text-blue-500" />
            <TrendingUp className="w-5 h-5 text-green-500" />
          </div>
          <h3 className="text-2xl font-bold text-gray-900">{metrics ? metrics.accuracy : '--'}%</h3>
          <p className="text-gray-600">Accuracy</p>
        </div>

        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-4">
            <BarChart3 className="w-8 h-8 text-purple-500" />
            <TrendingUp className="w-5 h-5 text-green-500" />
          </div>
          <h3 className="text-2xl font-bold text-gray-900">{metrics ? metrics.precision : '--'}%</h3>
          <p className="text-gray-600">Precision</p>
        </div>

        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-4">
            <BarChart3 className="w-8 h-8 text-green-500" />
            <TrendingUp className="w-5 h-5 text-green-500" />
          </div>
          <h3 className="text-2xl font-bold text-gray-900">{metrics ? metrics.recall : '--'}%</h3>
          <p className="text-gray-600">Recall</p>
        </div>

        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-4">
            <BarChart3 className="w-8 h-8 text-amber-500" />
            <TrendingUp className="w-5 h-5 text-green-500" />
          </div>
          <h3 className="text-2xl font-bold text-gray-900">{metrics ? metrics.f1Score : '--'}%</h3>
          <p className="text-gray-600">F1 Score</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Training Data */}
        <div className="lg:col-span-2 bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-semibold text-gray-900">Training Data</h2>
            <button
              onClick={handleStartTraining}
              disabled={isTraining}
              className="flex items-center space-x-2 px-4 py-2 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl hover:shadow-lg transition-all disabled:opacity-50"
            >
              <Play className="w-4 h-4" />
              <span>{isTraining ? 'Training...' : 'Start Training'}</span>
            </button>
          </div>

          <div className="space-y-4">
            {trainingData.map((item) => (
              <div key={item.id} className="bg-white/50 rounded-xl p-4 border border-white/30">
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-3 flex-1">
                    {getStatusIcon(item.status)}
                    <div className="flex-1">
                      <p className="font-medium text-gray-900">{item.filename}</p>
                      <div className="flex items-center space-x-4 mt-1">
                        <select
                          value={item.label}
                          onChange={(e) => handleLabelChange(item.id, e.target.value)}
                          className="text-sm bg-white/80 border border-gray-300 rounded-lg px-2 py-1"
                          disabled={item.status === 'training'}
                        >
                          {documentTypes.map(type => (
                            <option key={type} value={type}>{type}</option>
                          ))}
                        </select>
                        <span className="text-sm text-gray-500">{item.confidence}% confidence</span>
                      </div>
                    </div>
                  </div>
                  <div className="flex items-center space-x-2">
                    {item.status === 'pending' && (
                      <button
                        onClick={() => handleValidateData(item.id)}
                        className="px-3 py-1 bg-green-100 text-green-700 rounded-lg hover:bg-green-200 transition-colors text-sm"
                      >
                        Validate
                      </button>
                    )}
                    <button
                      onClick={() => handleRemoveData(item.id)}
                      className="p-1 hover:bg-red-100 rounded-lg transition-colors"
                      disabled={item.status === 'training'}
                    >
                      <X className="w-4 h-4 text-gray-500 hover:text-red-500" />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Upload New Training Data */}
          <div className="mt-6 pt-6 border-t border-gray-200">
            <div className="border-2 border-dashed border-gray-300 rounded-xl p-6 text-center hover:border-blue-400 transition-colors">
              <Upload className="w-8 h-8 text-gray-400 mx-auto mb-2" />
              <p className="text-gray-600 mb-2">Upload labeled documents for training</p>
              <input
                type="file"
                multiple={false}
                accept=".pdf,.docx,.txt"
                className="hidden"
                id="training-upload"
                onChange={handleFileChange}
              />
              <label
                htmlFor="training-upload"
                className="inline-flex items-center space-x-2 px-4 py-2 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors cursor-pointer"
              >
                <Upload className="w-4 h-4" />
                <span>Choose File</span>
              </label>

            </div>
            {file && file.name && (
              <div className="mt-2 flex justify-center items-center space-x-2">
                <span className="text-xs text-gray-600">{file.name}</span>
                <select
                  value={label}
                  onChange={e => setLabel(e.target.value)}
                  className="text-sm bg-white/80 border border-gray-300 rounded-lg px-2 py-1"
                >
                  {documentTypes.map(type => (
                    <option key={type} value={type}>{type}</option>
                  ))}
                </select>
                <button
                  onClick={handleUpload}
                  disabled={!file || uploading}
                  className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors disabled:opacity-50"
                >
                  {uploading ? 'Uploading...' : 'Upload'}
                </button>
              </div>
            )}

          </div>
        </div>

        {/* Document Types Management */}
        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-xl font-semibold text-gray-900">Document Types</h2>
            <button
              onClick={() => setShowAddLabel(true)}
              className="p-2 bg-blue-100 text-blue-600 rounded-lg hover:bg-blue-200 transition-colors"
            >
              <Plus className="w-4 h-4" />
            </button>
          </div>

          <div className="space-y-2 mb-6">
            {documentTypes.map((type, index) => (
              <div key={index} className="flex items-center justify-between p-3 bg-white/50 rounded-lg border border-white/30">
                <span className="font-medium text-gray-900">{type}</span>
                <span className="text-sm text-gray-500">
                  {trainingData.filter(item => item.label === type).length} samples
                </span>
              </div>
            ))}
          </div>

          {showAddLabel && (
            <div className="space-y-3">
              <input
                type="text"
                value={newLabel}
                onChange={(e) => setNewLabel(e.target.value)}
                placeholder="Enter new document type"
                className="w-full px-3 py-2 bg-white/80 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
              <div className="flex space-x-2">
                <button
                  onClick={addNewLabel}
                  className="flex-1 px-3 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 transition-colors"
                >
                  Add
                </button>
                <button
                  onClick={() => {
                    setShowAddLabel(false);
                    setNewLabel('');
                  }}
                  className="flex-1 px-3 py-2 bg-gray-300 text-gray-700 rounded-lg hover:bg-gray-400 transition-colors"
                >
                  Cancel
                </button>
              </div>
            </div>
          )}

          {/* Training Progress */}
          {isTraining && (
            <div className="mt-6 pt-6 border-t border-gray-200">
              <div className="flex items-center space-x-3 mb-3">
                <GraduationCap className="w-6 h-6 text-blue-500" />
                <span className="font-medium text-gray-900">Training in Progress</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2 mb-2">
                <div
                  className="bg-gradient-to-r from-blue-500 to-purple-600 h-2 rounded-full animate-pulse"
                  style={{ width: progress && progress.totalDocuments > 0 ? `${Math.round((progress.processedDocuments / progress.totalDocuments) * 100)}%` : '0%' }}
                />
              </div>
              <div className="flex justify-between text-sm text-gray-600">
                <span>
                  {progress
                    ? `${progress.processedDocuments} / ${progress.totalDocuments} documents`
                    : '--'}
                </span>
                <span>
                  {progress && progress.estimatedTimeSeconds !== null
                    ? `Estimated time: ${Math.ceil(progress.estimatedTimeSeconds / 60)} min`
                    : ''}
                </span>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};