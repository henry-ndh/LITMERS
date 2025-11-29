import { useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import WorkspaceLayout from '@/components/layout/workspace-layout';
import ProjectSidebar from './components/ProjectSidebar';
import KanbanBoard from '@/pages/IssuePage/components/KanbanBoard';
import ProjectMembersManagement from './components/ProjectMembersManagement';
import ProjectLabelsManagement from './components/ProjectLabelsManagement';
import IssueStatusManagement from './components/IssueStatusManagement';
import { useGetProjectDetail } from '@/queries/project.query';
import { Tabs, TabsList, TabsTrigger, TabsContent } from '@/components/ui/tabs';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Search, Filter, Users } from 'lucide-react';
import __helpers from '@/helpers';

export default function WorkspacePage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const currentUserId = __helpers.getUserId();

  const teamId = searchParams.get('teamId');
  const projectId = searchParams.get('projectId');

  const { data: projectData } = useGetProjectDetail(projectId || undefined);
  const project = projectData?.data || projectData;

  // Check if user is owner - handle both number and string comparison
  const projectOwnerId = project?.ownerId?.toString();
  const userId = currentUserId?.toString();
  const canManage = Boolean(
    projectOwnerId && userId && projectOwnerId === userId
  );
  const handleTeamChange = (newTeamId: number) => {
    setSearchParams({ teamId: newTeamId.toString() });
  };

  const handleProjectSelect = (selectedProjectId: number) => {
    if (teamId) {
      setSearchParams({
        teamId: teamId,
        projectId: selectedProjectId.toString()
      });
    }
  };

  useEffect(() => {
    // This will be handled by the layout component
  }, []);

  return (
    <WorkspaceLayout
      selectedTeamId={teamId ? Number(teamId) : undefined}
      onTeamChange={handleTeamChange}
    >
      {teamId ? (
        <>
          {/* Sidebar */}
          <ProjectSidebar
            teamId={Number(teamId)}
            selectedProjectId={projectId ? Number(projectId) : undefined}
            onProjectSelect={handleProjectSelect}
            canManage={true}
          />

          {/* Main Content */}
          {projectId ? (
            <div className="flex flex-1 flex-col overflow-hidden">
              <Tabs defaultValue="board" className="flex flex-1 flex-col">
                {/* Top Bar */}
                <div className="border-b bg-white">
                  <div className="flex items-center justify-between px-6 py-3">
                    <div className="flex items-center gap-4">
                      <div className="flex items-center gap-2">
                        <h1 className="text-lg font-semibold">
                          {project?.name || 'Project'}
                        </h1>
                      </div>
                      <TabsList>
                        <TabsTrigger value="board">Board</TabsTrigger>
                        <TabsTrigger value="members">
                          <Users className="mr-2 h-4 w-4" />
                          Members
                        </TabsTrigger>
                        {canManage && (
                          <TabsTrigger value="settings">Settings</TabsTrigger>
                        )}
                      </TabsList>
                    </div>
                  </div>
                </div>

                {/* Tab Content */}
                <div className="flex-1 overflow-auto bg-slate-50">
                  <TabsContent value="board" className="m-0 h-full p-0">
                    <div className="flex h-full flex-col">
                      {/* Board Controls */}
                      <div className="flex items-center gap-2 border-b bg-white px-6 py-2">
                        <div className="relative max-w-md flex-1">
                          <Search className="absolute left-2 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                          <Input
                            placeholder="Search board"
                            className="h-8 pl-8"
                          />
                        </div>
                        <div className="flex items-center gap-2">
                          <Button variant="outline" size="sm" className="gap-2">
                            <Filter className="h-4 w-4" />
                            Filter
                          </Button>
                        </div>
                      </div>

                      {/* Kanban Board */}
                      <div className="flex-1 overflow-hidden">
                        <KanbanBoard
                          projectId={Number(projectId)}
                          canManage={canManage}
                        />
                      </div>
                    </div>
                  </TabsContent>

                  <TabsContent value="members" className="m-0 h-full p-6">
                    <ProjectMembersManagement
                      teamId={Number(teamId)}
                      canManage={canManage}
                    />
                  </TabsContent>

                  {canManage && (
                    <TabsContent
                      value="settings"
                      className="m-0 h-full space-y-6 p-6"
                    >
                      <ProjectLabelsManagement projectId={Number(projectId)} />
                      <IssueStatusManagement projectId={Number(projectId)} />
                    </TabsContent>
                  )}
                </div>
              </Tabs>
            </div>
          ) : (
            <div className="flex flex-1 items-center justify-center">
              <div className="text-center">
                <p className="text-muted-foreground">
                  Select a project to view issues
                </p>
              </div>
            </div>
          )}
        </>
      ) : (
        <div className="flex flex-1 items-center justify-center">
          <div className="text-center">
            <p className="text-muted-foreground">
              Select a team to get started
            </p>
          </div>
        </div>
      )}
    </WorkspaceLayout>
  );
}
