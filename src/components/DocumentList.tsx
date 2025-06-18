import React, { useState, useEffect } from 'react';
import { 
  Search, 
  Filter, 
  Download, 
  Eye, 
  Trash2, 
  FileText,
  Calendar,
  Tag,
  SortAsc,
  MoreVertical,
  X
} from 'lucide-react';

interface Document {
  id: string;
  name: string;
  type: string;
  classification: {
    predicted: string;
    confidence: number;
  };
  uploadDate: Date;
  size: string;
  status: 'processed' | 'processing' | 'error';
}

export const DocumentList: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedType, setSelectedType] = useState('all');
  const [sortBy, setSortBy] = useState('date');
  const [selectedDocuments, setSelectedDocuments] = useState<string[]>([]);
  const [documents, setDocuments] = useState<Document[]>([]);
  const [isDocumentsLoading, setIsDocumentsLoading] = useState(true);
  const [documentsError, setDocumentsError] = useState<string | null>(null);
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
  const [previewDoc, setPreviewDoc] = useState<Document | null>(null);
  const [openMenuId, setOpenMenuId] = useState<string | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);

  useEffect(() => {
    const fetchDocuments = async () => {
      setIsDocumentsLoading(true);
      setDocumentsError(null);
      try {
        const response = await fetch(`${apiBaseUrl}/documents`);
        if (!response.ok) {
          throw new Error('Failed to fetch documents');
        }
        const data = await response.json();
        setDocuments(
          data.map((doc: any) => ({
            id: doc.id,
            name: doc.fileName,
            type: doc.predictedType,
            classification: {
              predicted: doc.predictedType,
              confidence: doc.confidence ? Math.round(doc.confidence * 100) : 0
            },
            uploadDate: new Date(doc.uploadDate),
            size: doc.fileSize ?
              (doc.fileSize > 1024 * 1024
                ? `${(doc.fileSize / 1024 / 1024).toFixed(2)} MB`
                : `${(doc.fileSize / 1024).toFixed(0)} KB`)
              : 'Unknown',
            status: (doc.status || '').toLowerCase() === 'processed' ? 'processed' :
                   (doc.status || '').toLowerCase() === 'processing' ? 'processing' : 'error',
          }))
        );
      } catch (error: any) {
        setDocumentsError(error.message || 'Unknown error');
      } finally {
        setIsDocumentsLoading(false);
      }
    };
    fetchDocuments();
  }, [apiBaseUrl]);

  const documentTypes = ['all', 'Invoice', 'Resume', 'Contract', 'Purchase Order', 'Agreement', 'Report'];

  const filteredDocuments = documents.filter(doc => {
    const matchesSearch = doc.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         doc.classification.predicted.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesType = selectedType === 'all' || doc.classification.predicted === selectedType;
    return matchesSearch && matchesType;
  });

  const handleDocumentSelect = (id: string) => {
    setSelectedDocuments(prev => 
      prev.includes(id) 
        ? prev.filter(docId => docId !== id)
        : [...prev, id]
    );
  };

  const handleSelectAll = () => {
    setSelectedDocuments(
      selectedDocuments.length === filteredDocuments.length 
        ? [] 
        : filteredDocuments.map(doc => doc.id)
    );
  };

  const getStatusBadge = (status: Document['status']) => {
    switch (status) {
      case 'processed':
        return <span className="px-2 py-1 bg-green-100 text-green-700 rounded-full text-xs font-medium">Processed</span>;
      case 'processing':
        return <span className="px-2 py-1 bg-blue-100 text-blue-700 rounded-full text-xs font-medium">Processing</span>;
      case 'error':
        return <span className="px-2 py-1 bg-red-100 text-red-700 rounded-full text-xs font-medium">Error</span>;
    }
  };

  const getConfidenceColor = (confidence: number) => {
    if (confidence >= 95) return 'text-green-600';
    if (confidence >= 85) return 'text-yellow-600';
    return 'text-red-600';
  };

  // Download document handler
  const handleDownload = async (doc: Document) => {
    try {
      const response = await fetch(`${apiBaseUrl}/documents/${doc.id}/download`);
      if (!response.ok) throw new Error('Failed to download');
      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = doc.name;
      document.body.appendChild(a);
      a.click();
      a.remove();
      window.URL.revokeObjectURL(url);
    } catch (err) {
      alert('Download failed');
    }
  };

  // Delete document handler
  const handleDelete = async (doc: Document) => {
    if (!window.confirm(`Delete document "${doc.name}"?`)) return;
    setDeletingId(doc.id);
    try {
      const response = await fetch(`${apiBaseUrl}/documents/${doc.id}`, { method: 'DELETE' });
      if (!response.ok) throw new Error('Failed to delete');
      setDocuments(prev => prev.filter(d => d.id !== doc.id));
      setOpenMenuId(null);
    } catch (err) {
      alert('Delete failed');
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Document Library</h1>
        <p className="text-gray-600">Manage and search your classified documents</p>
      </div>

      {/* Search and Filters */}
      <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
        <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between space-y-4 lg:space-y-0 lg:space-x-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search documents..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-3 bg-white/60 border border-white/30 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          
          <div className="flex items-center space-x-3">
            <div className="flex items-center space-x-2">
              <Filter className="w-5 h-5 text-gray-500" />
              <select
                value={selectedType}
                onChange={(e) => setSelectedType(e.target.value)}
                className="bg-white/80 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                {documentTypes.map(type => (
                  <option key={type} value={type}>
                    {type === 'all' ? 'All Types' : type}
                  </option>
                ))}
              </select>
            </div>
            
            <div className="flex items-center space-x-2">
              <SortAsc className="w-5 h-5 text-gray-500" />
              <select
                value={sortBy}
                onChange={(e) => setSortBy(e.target.value)}
                className="bg-white/80 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="date">Sort by Date</option>
                <option value="name">Sort by Name</option>
                <option value="type">Sort by Type</option>
                <option value="confidence">Sort by Confidence</option>
              </select>
            </div>
          </div>
        </div>

        {selectedDocuments.length > 0 && (
          <div className="mt-4 pt-4 border-t border-gray-200">
            <div className="flex items-center justify-between">
              <span className="text-sm text-gray-600">
                {selectedDocuments.length} document{selectedDocuments.length !== 1 ? 's' : ''} selected
              </span>
              <div className="flex items-center space-x-2">
                <button className="flex items-center space-x-1 px-3 py-1 bg-blue-100 text-blue-700 rounded-lg hover:bg-blue-200 transition-colors">
                  <Download className="w-4 h-4" />
                  <span className="text-sm">Download</span>
                </button>
                <button className="flex items-center space-x-1 px-3 py-1 bg-red-100 text-red-700 rounded-lg hover:bg-red-200 transition-colors">
                  <Trash2 className="w-4 h-4" />
                  <span className="text-sm">Delete</span>
                </button>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Document Grid */}
      <div className="bg-white/70 backdrop-blur-sm rounded-2xl border border-white/20 shadow-lg overflow-hidden">
        <div className="p-6 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-gray-900">
              {filteredDocuments.length} Document{filteredDocuments.length !== 1 ? 's' : ''}
            </h2>
            <label className="flex items-center space-x-2 cursor-pointer">
              <input
                type="checkbox"
                checked={selectedDocuments.length === filteredDocuments.length && filteredDocuments.length > 0}
                onChange={handleSelectAll}
                className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
              />
              <span className="text-sm text-gray-600">Select all</span>
            </label>
          </div>
        </div>

        <div className="divide-y divide-gray-200">
          {isDocumentsLoading ? (
            <div className="p-12 text-center text-gray-500">Loading documents...</div>
          ) : documentsError ? (
            <div className="p-12 text-center text-red-500">{documentsError}</div>
          ) : filteredDocuments.map((doc) => (
            <div key={doc.id} className="p-6 hover:bg-white/30 transition-colors">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-4">
                  <input
                    type="checkbox"
                    checked={selectedDocuments.includes(doc.id)}
                    onChange={() => handleDocumentSelect(doc.id)}
                    className="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <div className="p-2 bg-blue-100 rounded-lg">
                    <FileText className="w-6 h-6 text-blue-600" />
                  </div>
                  <div className="flex-1">
                    <h3 className="font-medium text-gray-900 mb-1">{doc.name}</h3>
                    <div className="flex items-center space-x-4 text-sm text-gray-500">
                      <div className="flex items-center space-x-1">
                        <Tag className="w-4 h-4" />
                        <span className="font-medium text-blue-600">{doc.classification.predicted}</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        <span className={`font-medium ${getConfidenceColor(doc.classification.confidence)}`}>{doc.classification.confidence}%</span>
                      </div>
                      <div className="flex items-center space-x-1">
                        <Calendar className="w-4 h-4" />
                        <span>{doc.uploadDate.toLocaleDateString()}</span>
                      </div>
                      <span>{doc.size}</span>
                    </div>
                  </div>
                </div>

                <div className="flex items-center space-x-3">
                  {getStatusBadge(doc.status)}
                  <div className="flex items-center space-x-2">
                    <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Preview" onClick={() => setPreviewDoc(doc)}>
                      <Eye className="w-4 h-4 text-gray-500" />
                    </button>
                    <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="Download" onClick={() => handleDownload(doc)}>
                      <Download className="w-4 h-4 text-gray-500" />
                    </button>
                    <div className="relative">
                      <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors" title="More options" onClick={() => setOpenMenuId(openMenuId === doc.id ? null : doc.id)}>
                        <MoreVertical className="w-4 h-4 text-gray-500" />
                      </button>
                      {openMenuId === doc.id && (
                        <div className="absolute right-0 mt-2 w-32 bg-white border border-gray-200 rounded-lg shadow-lg z-10">
                          <button
                            className="flex items-center w-full px-4 py-2 text-red-600 hover:bg-red-50"
                            onClick={() => handleDelete(doc)}
                            disabled={deletingId === doc.id}
                          >
                            <Trash2 className="w-4 h-4 mr-2" />
                            {deletingId === doc.id ? 'Deleting...' : 'Delete'}
                          </button>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </div>

        {filteredDocuments.length === 0 && (
          <div className="p-12 text-center">
            <FileText className="w-12 h-12 text-gray-400 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No documents found</h3>
            <p className="text-gray-500">Try adjusting your search or filter criteria</p>
          </div>
        )}
      </div>

      {/* Preview Modal */}
      {previewDoc && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-40">
          <div className="bg-white rounded-xl shadow-xl max-w-lg w-full p-6 relative">
            <button className="absolute top-3 right-3 p-1 rounded hover:bg-gray-200" onClick={() => setPreviewDoc(null)}>
              <X className="w-5 h-5 text-gray-500" />
            </button>
            <h2 className="text-xl font-semibold mb-2">{previewDoc.name}</h2>
            <div className="mb-2 text-sm text-gray-600">Type: {previewDoc.classification.predicted}</div>
            <div className="mb-2 text-sm text-gray-600">Confidence: {previewDoc.classification.confidence}%</div>
            <div className="mb-2 text-sm text-gray-600">Uploaded: {previewDoc.uploadDate.toLocaleDateString()}</div>
            <div className="mb-2 text-sm text-gray-600">Size: {previewDoc.size}</div>
            {/* Optionally, show more details or extracted text here */}
          </div>
        </div>
      )}
    </div>
  );
};