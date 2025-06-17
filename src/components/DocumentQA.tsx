import React, { useState, useEffect } from 'react';
import { 
  MessageCircle, 
  Send, 
  Bot, 
  User, 
  FileText,
  Sparkles,
  Clock
} from 'lucide-react';

interface Message {
  id: string;
  type: 'user' | 'assistant';
  content: string;
  timestamp: Date;
  relatedDocument?: string;
}

interface Document {
  id: string;
  name: string;
  type: string;
  uploadDate: Date;
}

export const DocumentQA: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([
    {
      id: '1',
      type: 'assistant',
      content: 'Hello! I can help you extract information and answer questions about your uploaded documents. Select a document and ask me anything!',
      timestamp: new Date(),
    }
  ]);
  const [inputMessage, setInputMessage] = useState('');
  const [selectedDocument, setSelectedDocument] = useState<string>('');
  const [isLoading, setIsLoading] = useState(false);
  const [documents, setDocuments] = useState<Document[]>([]);
  const [isDocumentsLoading, setIsDocumentsLoading] = useState(true);
  const [documentsError, setDocumentsError] = useState<string | null>(null);
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
  

  const predefinedQuestions = [
    'Who signed this document?',
  ];

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
        // Assuming the API returns an array of documents with id, name, type, uploadDate
        setDocuments(
          data.map((doc: any) => ({
            id: doc.id,
            name: doc.fileName,
            type: doc.predictedType,
            uploadDate: new Date(doc.uploadDate)
          }))
        );
      } catch (error: any) {
        setDocumentsError(error.message || 'Unknown error');
      } finally {
        setIsDocumentsLoading(false);
      }
    };
    fetchDocuments();
  }, []);

  const handleSendMessage = async () => {
    if (!inputMessage.trim() || !selectedDocument) return;

    const userMessage: Message = {
      id: Date.now().toString(),
      type: 'user',
      content: inputMessage,
      timestamp: new Date(),
      relatedDocument: selectedDocument
    };

    setMessages(prev => [...prev, userMessage]);
    setInputMessage('');
    setIsLoading(true);

    try {
      const response = await fetch(`${apiBaseUrl}/QA/ask`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          question: inputMessage,
          documentId: selectedDocument,
        }),
      });
      if (!response.ok) {
        throw new Error('Failed to get answer from the server');
      }
      const data = await response.json();
      const assistantMessage: Message = {
        id: (Date.now() + 1).toString(),
        type: 'assistant',
        content: data.answer || 'No answer returned.',
        timestamp: new Date(),
        relatedDocument: selectedDocument
      };
      setMessages(prev => [...prev, assistantMessage]);
    } catch (error: any) {
      const assistantMessage: Message = {
        id: (Date.now() + 1).toString(),
        type: 'assistant',
        content: error.message || 'An error occurred while getting the answer.',
        timestamp: new Date(),
        relatedDocument: selectedDocument
      };
      setMessages(prev => [...prev, assistantMessage]);
    } finally {
      setIsLoading(false);
    }
  };

  const handleQuestionClick = (question: string) => {
    setInputMessage(question);
  };

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Document Q&A</h1>
        <p className="text-gray-600">Ask questions about your documents using AI</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-8">
        {/* Document Selection */}
        <div className="lg:col-span-1">
          <div className="bg-white/70 backdrop-blur-sm rounded-2xl p-6 border border-white/20 shadow-lg">
            <h2 className="text-lg font-semibold text-gray-900 mb-4">Select Document</h2>
            <div className="space-y-2">
              {isDocumentsLoading ? (
                <div className="text-gray-500 text-sm">Loading documents...</div>
              ) : documentsError ? (
                <div className="text-red-500 text-sm">{documentsError}</div>
              ) : (
                documents.map((doc) => (
                  <button
                    key={doc.id}
                    onClick={() => setSelectedDocument(doc.id)}
                    className={`w-full text-left p-3 rounded-xl border transition-all ${
                      selectedDocument === doc.id
                        ? 'border-blue-500 bg-blue-50/50'
                        : 'border-gray-200 hover:border-blue-300 hover:bg-blue-50/30'
                    }`}
                  >
                    <div className="flex items-center space-x-2 mb-1">
                      <FileText className="w-4 h-4 text-blue-500" />
                      <span className="font-medium text-sm text-gray-900 truncate">{doc.name}</span>
                    </div>
                    <div className="flex justify-between items-center">
                      <span className="text-xs text-blue-600">{doc.type}</span>
                      <span className="text-xs text-gray-500">
                        {doc.uploadDate.toLocaleDateString()}
                      </span>
                    </div>
                  </button>
                ))
              )}
            </div>

            {selectedDocument && (
              <div className="mt-6 pt-4 border-t border-gray-200">
                <h3 className="text-sm font-semibold text-gray-900 mb-3">Quick Questions</h3>
                <div className="space-y-2">
                  {predefinedQuestions.map((question, index) => (
                    <button
                      key={index}
                      onClick={() => handleQuestionClick(question)}
                      className="w-full text-left text-xs p-2 bg-gray-100 hover:bg-gray-200 rounded-lg transition-colors"
                    >
                      {question}
                    </button>
                  ))}
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Chat Interface */}
        <div className="lg:col-span-3">
          <div className="bg-white/70 backdrop-blur-sm rounded-2xl border border-white/20 shadow-lg h-[600px] flex flex-col">
            {/* Chat Header */}
            <div className="p-6 border-b border-gray-200 flex items-center space-x-3">
              <div className="p-2 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
                <Sparkles className="w-5 h-5 text-white" />
              </div>
              <div>
                <h3 className="font-semibold text-gray-900">AI Document Assistant</h3>
                <p className="text-sm text-gray-500">
                  {selectedDocument ? 'Ready to answer questions' : 'Select a document to start'}
                </p>
              </div>
            </div>

            {/* Messages */}
            <div className="flex-1 overflow-y-auto p-6 space-y-4">
              {messages.map((message) => (
                <div
                  key={message.id}
                  className={`flex ${message.type === 'user' ? 'justify-end' : 'justify-start'}`}
                >
                  <div className={`max-w-[65%] flex items-start space-x-3 ${
                    message.type === 'user' ? 'flex-row-reverse space-x-reverse' : ''
                  }`}>
                    <div className={`p-2 rounded-xl flex-shrink-0 ${
                      message.type === 'user' 
                        ? 'bg-blue-500' 
                        : 'bg-gradient-to-br from-purple-500 to-pink-600'
                    }`}>
                      {message.type === 'user' ? (
                        <User className="w-4 h-4 text-white" />
                      ) : (
                        <Bot className="w-4 h-4 text-white" />
                      )}
                    </div>
                    <div className={`rounded-2xl p-4 ${
                      message.type === 'user'
                        ? 'bg-blue-500 text-white'
                        : 'bg-white/80 text-gray-900 border border-white/30'
                    }`}>
                      <p className="text-sm">{message.content}</p>
                      <div className="flex items-center space-x-1 mt-2 opacity-70">
                        <Clock className="w-3 h-3" />
                        <span className="text-xs">
                          {message.timestamp.toLocaleTimeString([], { 
                            hour: '2-digit', 
                            minute: '2-digit' 
                          })}
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              ))}

              {isLoading && (
                <div className="flex justify-start">
                  <div className="flex items-start space-x-3">
                    <div className="p-2 rounded-xl bg-gradient-to-br from-purple-500 to-pink-600">
                      <Bot className="w-4 h-4 text-white" />
                    </div>
                    <div className="bg-white/80 rounded-2xl p-4 border border-white/30">
                      <div className="flex items-center space-x-2">
                        <div className="flex space-x-1">
                          <div className="w-2 h-2 bg-purple-500 rounded-full animate-bounce"></div>
                          <div className="w-2 h-2 bg-purple-500 rounded-full animate-bounce" style={{ animationDelay: '0.1s' }}></div>
                          <div className="w-2 h-2 bg-purple-500 rounded-full animate-bounce" style={{ animationDelay: '0.2s' }}></div>
                        </div>
                        <span className="text-sm text-gray-600">Analyzing document...</span>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>

            {/* Input */}
            <div className="p-6 border-t border-gray-200">
              <div className="flex space-x-3">
                <input
                  type="text"
                  value={inputMessage}
                  onChange={(e) => setInputMessage(e.target.value)}
                  onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
                  placeholder={selectedDocument ? "Ask a question about the document..." : "Please select a document first"}
                  disabled={!selectedDocument || isLoading}
                  className="flex-1 px-4 py-3 bg-white/60 border border-white/30 rounded-xl focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:opacity-50"
                />
                <button
                  onClick={handleSendMessage}
                  disabled={!inputMessage.trim() || !selectedDocument || isLoading}
                  className="px-6 py-3 bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-xl hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <Send className="w-5 h-5" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};