import React, { useState } from 'react';
import { Sidebar } from './components/Sidebar';
import { Dashboard } from './components/Dashboard';
import { DocumentUpload } from './components/DocumentUpload';
import { DocumentQA } from './components/DocumentQA';
import { TrainingModule } from './components/TrainingModule';
import { DocumentList } from './components/DocumentList';

export type ActiveView = 'dashboard' | 'upload' | 'qa' | 'training' | 'documents';

function App() {
  const [activeView, setActiveView] = useState<ActiveView>('dashboard');

  const renderActiveView = () => {
    switch (activeView) {
      case 'dashboard':
        return <Dashboard />;
      case 'upload':
        return <DocumentUpload />;
      case 'qa':
        return <DocumentQA />;
      case 'training':
        return <TrainingModule />;
      case 'documents':
        return <DocumentList />;
      default:
        return <Dashboard />;
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-indigo-100">
      <div className="flex">
        <Sidebar activeView={activeView} setActiveView={setActiveView} />
        <main className="flex-1 ml-64">
          <div className="p-8">
            {renderActiveView()}
          </div>
        </main>
      </div>
    </div>
  );
}

export default App;