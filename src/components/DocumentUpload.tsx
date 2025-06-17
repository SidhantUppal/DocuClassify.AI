import React, { useState, useCallback } from 'react';
import { 
  Upload, 
  FileText, 
  CheckCircle, 
  X, 
  Loader2,
  AlertCircle,
  Download
} from 'lucide-react';

interface UploadedDocument {
  id: string;
  name: string;
  size: string;
  type: string;
  classification?: {
    predicted: string;
    confidence: number;
    alternatives: Array<{ type: string; confidence: number }>;
  };
  status: 'uploading' | 'processing' | 'completed' | 'error';
}

export const DocumentUpload: React.FC = () => {
  const [documents, setDocuments] = useState<UploadedDocument[]>([]);
  const [isDragOver, setIsDragOver] = useState(false);
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

  const simulateProcessing = (doc: UploadedDocument) => {
    // Simulate upload
    setTimeout(() => {
      setDocuments(prev => prev.map(d => 
        d.id === doc.id ? { ...d, status: 'processing' as const } : d
      ));

      // Simulate ML classification
      setTimeout(() => {
        const classifications = [
          { predicted: 'Invoice', confidence: 98.5, alternatives: [
            { type: 'Receipt', confidence: 85.2 },
            { type: 'Purchase Order', confidence: 76.8 }
          ]},
          { predicted: 'Resume', confidence: 95.7, alternatives: [
            { type: 'Cover Letter', confidence: 78.3 },
            { type: 'Reference Letter', confidence: 65.1 }
          ]},
          { predicted: 'Contract', confidence: 92.3, alternatives: [
            { type: 'Agreement', confidence: 88.9 },
            { type: 'Legal Document', confidence: 74.2 }
          ]}
        ];

        const randomClassification = classifications[Math.floor(Math.random() * classifications.length)];

        setDocuments(prev => prev.map(d => 
          d.id === doc.id 
            ? { ...d, status: 'completed' as const, classification: randomClassification }
            : d
        ));
      }, 2000);
    }, 1000);
  };

  const uploadDocumentToAPI = async (file: File) => {
    debugger;
    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch(`${apiBaseUrl}/documents/upload`, { // Change to your actual API endpoint
        method: 'POST',
        body: formData,
      });

      if (!response.ok) {
        throw new Error('Upload failed');
      }

      // Optionally, parse response if your API returns classification, etc.
      return await response.json();
    } catch (error) {
      console.error(error);
      return { error: true };
    }
  };

  const handleFileUpload = useCallback((files: FileList) => {
    debugger;
    Array.from(files).forEach(async (file) => {
      const newDoc: UploadedDocument = {
        id: Math.random().toString(36).substr(2, 9),
        name: file.name,
        size: (file.size / 1024 / 1024).toFixed(2) + ' MB',
        type: file.type,
        status: 'uploading'
      };
  
      setDocuments(prev => [...prev, newDoc]);
  
      // Call the API
      const result = await uploadDocumentToAPI(file);
  
      if (result && !result.error) {
        // If your API returns classification, use it here
        setDocuments(prev => prev.map(d =>
          d.id === newDoc.id
            ? {
                ...d,
                status: 'completed',
                classification: result.classification // adjust based on your API response
              }
            : d
        ));
      } else {
        setDocuments(prev => prev.map(d =>
          d.id === newDoc.id
            ? { ...d, status: 'error' }
            : d
        ));
      }
    });
  }, []);

  const handleDragOver = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(true);
  }, []);

  const handleDragLeave = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
  }, []);

  const handleDrop = useCallback((e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
    if (e.dataTransfer.files) {
      handleFileUpload(e.dataTransfer.files);
    }
  }, [handleFileUpload]);

  const removeDocument = (id: string) => {
    setDocuments(prev => prev.filter(d => d.id !== id));
  };

  const getStatusIcon = (status: UploadedDocument['status']) => {
    switch (status) {
      case 'uploading':
      case 'processing':
        return <Loader2 className="w-5 h-5 text-blue-500 animate-spin" />;
      case 'completed':
        return <CheckCircle className="w-5 h-5 text-green-500" />;
      case 'error':
        return <AlertCircle className="w-5 h-5 text-red-500" />;
    }
  };

  const getStatusText = (status: UploadedDocument['status']) => {
    switch (status) {
      case 'uploading':
        return 'Uploading...';
      case 'processing':
        return 'Classifying...';
      case 'completed':
        return 'Completed';
      case 'error':
        return 'Error';
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Document Upload</h1>
        <p className="text-gray-600">Upload documents for AI-powered classification</p>
      </div>

      {/* Upload Area */}
      <div 
        className={`relative border-2 border-dashed rounded-2xl p-12 text-center transition-all duration-300 ${
          isDragOver 
            ? 'border-blue-500 bg-blue-50/50' 
            : 'border-gray-300 bg-white/50 hover:border-blue-400 hover:bg-blue-50/30'
        }`}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
      >
        <div className="space-y-4">
          <div className="mx-auto w-16 h-16 bg-gradient-to-br from-blue-500 to-purple-600 rounded-2xl flex items-center justify-center">
            <Upload className="w-8 h-8 text-white" />
          </div>
          <div>
            <h3 className="text-xl font-semibold text-gray-900 mb-2">Drop your documents here</h3>
            <p className="text-gray-600 mb-4">or click to browse files</p>
            <p className="text-sm text-gray-500">Supports PDF, DOCX, and TXT files up to 10MB</p>
          </div>
          <input
            type="file"
            multiple
            accept=".pdf,.docx,.txt"
            onChange={(e) => e.target.files && handleFileUpload(e.target.files)}
            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
          />
        </div>
      </div>

      {/* Uploaded Documents */}
      {documents.length > 0 && (
        <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
          <h2 className="text-xl font-semibold text-gray-900 mb-6">Processing Queue</h2>
          <div className="space-y-4">
            {documents.map((doc) => (
              <div key={doc.id} className="bg-white/50 rounded-xl p-4 border border-white/30">
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-3">
                    <FileText className="w-8 h-8 text-blue-500" />
                    <div>
                      <p className="font-medium text-gray-900">{doc.name}</p>
                      <p className="text-sm text-gray-500">{doc.size}</p>
                    </div>
                  </div>
                  
                  <div className="flex items-center space-x-4">
                    <div className="flex items-center space-x-2">
                      {getStatusIcon(doc.status)}
                      <span className="text-sm font-medium">{getStatusText(doc.status)}</span>
                    </div>
                    <button
                      onClick={() => removeDocument(doc.id)}
                      className="p-1 hover:bg-red-100 rounded-lg transition-colors"
                    >
                      <X className="w-4 h-4 text-gray-500 hover:text-red-500" />
                    </button>
                  </div>
                </div>

                {doc.classification && (
                  <div className="mt-4 pt-4 border-t border-gray-200">
                    <div className="flex items-center justify-between mb-3">
                      <div>
                        <span className="text-lg font-semibold text-blue-600">{doc.classification.predicted}</span>
                        <span className="ml-2 text-sm text-green-600 font-medium">
                          {doc.classification.confidence}% confidence
                        </span>
                      </div>
                      <button className="flex items-center space-x-2 px-3 py-1 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors">
                        <Download className="w-4 h-4" />
                        <span className="text-sm">Export</span>
                      </button>
                    </div>
                    
                    <div className="space-y-2">
                      <p className="text-sm text-gray-600 font-medium">Alternative Classifications:</p>
                      <div className="flex flex-wrap gap-2">
                        {doc.classification.alternatives.map((alt, index) => (
                          <span
                            key={index}
                            className="px-2 py-1 bg-gray-100 text-gray-700 rounded-lg text-xs"
                          >
                            {alt.type} ({alt.confidence}%)
                          </span>
                        ))}
                      </div>
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};