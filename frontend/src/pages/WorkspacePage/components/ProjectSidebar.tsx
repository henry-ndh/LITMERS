import { useState } from 'react';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Separator } from '@/components/ui/separator';
import {
  Folder,
  Plus,
  Search,
  Star,
  Archive,
  Loader2,
  FolderOpen
} from 'lucide-react';
import { useGetProjectsByTeamId } from '@/queries/project.query';
import CreateProjectDialog from '@/pages/ProjectPage/components/CreateProjectDialog';
import { cn } from '@/lib/utils';

interface ProjectSidebarProps {
  teamId: string | number;
  selectedProjectId?: string | number;
  onProjectSelect?: (projectId: number) => void;
  canManage?: boolean;
}

export default function ProjectSidebar({
  teamId,
  selectedProjectId,
  onProjectSelect,
  canManage = false
}: ProjectSidebarProps) {
  const [searchQuery, setSearchQuery] = useState('');
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);

  const { data: projectsData, isLoading } = useGetProjectsByTeamId(teamId);
  const projects = projectsData?.data || projectsData || [];

  const filteredProjects = projects.filter((project: any) =>
    project.name?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const activeProjects = filteredProjects.filter((p: any) => !p.isArchived);
  const archivedProjects = filteredProjects.filter((p: any) => p.isArchived);

  return (
    <div className="flex h-full w-64 flex-col border-r border-slate-200 bg-white">
      {/* Header */}
      <div className="border-b border-slate-200 bg-white px-4 py-3">
        <div className="mb-3 flex items-center justify-between">
          <h2 className="text-sm font-semibold text-slate-800">Projects</h2>
          {canManage && (
            <CreateProjectDialog
              teamId={Number(teamId)}
              open={isCreateDialogOpen}
              onOpenChange={setIsCreateDialogOpen}
            >
              <Button
                variant="ghost"
                size="sm"
                className="h-7 w-7 p-0 text-slate-600 hover:bg-slate-100 hover:text-slate-900"
              >
                <Plus className="h-4 w-4" />
              </Button>
            </CreateProjectDialog>
          )}
        </div>

        {/* Search */}
        <div className="relative">
          <Search className="absolute left-2.5 top-1/2 h-3.5 w-3.5 -translate-y-1/2 text-slate-400" />
          <Input
            placeholder="Search projects..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="h-8 border-slate-300 bg-slate-50 pl-8 text-sm focus-visible:ring-blue-500"
          />
        </div>
      </div>

      {/* Project List */}
      <ScrollArea className="flex-1">
        {isLoading ? (
          <div className="flex h-32 items-center justify-center">
            <div className="flex flex-col items-center gap-2">
              <Loader2 className="h-5 w-5 animate-spin text-blue-600" />
              <p className="text-xs text-slate-500">Loading projects...</p>
            </div>
          </div>
        ) : (
          <div className="space-y-0.5 p-2">
            {/* Active Projects */}
            {activeProjects.length > 0 && (
              <div className="space-y-0.5">
                <div className="flex items-center gap-2 px-2 py-2">
                  <FolderOpen className="h-3.5 w-3.5 text-slate-500" />
                  <span className="text-xs font-semibold uppercase tracking-wider text-slate-600">
                    Active Projects
                  </span>
                  <span className="ml-auto text-xs text-slate-400">
                    {activeProjects.length}
                  </span>
                </div>
                {activeProjects.map((project: any) => (
                  <button
                    key={project.id}
                    onClick={() => onProjectSelect?.(project.id)}
                    className={cn(
                      'flex w-full items-center gap-2.5 rounded-md px-2.5 py-2 text-left text-sm transition-all',
                      'hover:bg-slate-100',
                      selectedProjectId?.toString() === project.id.toString()
                        ? 'bg-blue-50 font-medium text-blue-700 shadow-sm hover:bg-blue-100'
                        : 'text-slate-700'
                    )}
                  >
                    <div
                      className={cn(
                        'flex h-6 w-6 flex-shrink-0 items-center justify-center rounded',
                        selectedProjectId?.toString() === project.id.toString()
                          ? 'bg-blue-100'
                          : 'bg-slate-100'
                      )}
                    >
                      <Folder
                        className={cn(
                          'h-3.5 w-3.5',
                          selectedProjectId?.toString() ===
                            project.id.toString()
                            ? 'text-blue-700'
                            : 'text-slate-600'
                        )}
                      />
                    </div>
                    <span className="flex-1 truncate">{project.name}</span>
                    {project.isFavorite && (
                      <Star className="h-3.5 w-3.5 flex-shrink-0 fill-amber-400 text-amber-400" />
                    )}
                  </button>
                ))}
              </div>
            )}

            {/* Separator between active and archived */}
            {activeProjects.length > 0 && archivedProjects.length > 0 && (
              <Separator className="my-2" />
            )}

            {/* Archived Projects */}
            {archivedProjects.length > 0 && (
              <div className="space-y-0.5">
                <div className="flex items-center gap-2 px-2 py-2">
                  <Archive className="h-3.5 w-3.5 text-slate-400" />
                  <span className="text-xs font-semibold uppercase tracking-wider text-slate-500">
                    Archived
                  </span>
                  <span className="ml-auto text-xs text-slate-400">
                    {archivedProjects.length}
                  </span>
                </div>
                {archivedProjects.map((project: any) => (
                  <button
                    key={project.id}
                    onClick={() => onProjectSelect?.(project.id)}
                    className={cn(
                      'flex w-full items-center gap-2.5 rounded-md px-2.5 py-2 text-left text-sm transition-all',
                      'opacity-60 hover:bg-slate-100 hover:opacity-100',
                      selectedProjectId?.toString() === project.id.toString()
                        ? 'bg-blue-50 font-medium text-blue-700 hover:bg-blue-100'
                        : 'text-slate-600'
                    )}
                  >
                    <div
                      className={cn(
                        'flex h-6 w-6 flex-shrink-0 items-center justify-center rounded',
                        selectedProjectId?.toString() === project.id.toString()
                          ? 'bg-blue-100'
                          : 'bg-slate-100'
                      )}
                    >
                      <Folder
                        className={cn(
                          'h-3.5 w-3.5',
                          selectedProjectId?.toString() ===
                            project.id.toString()
                            ? 'text-blue-700'
                            : 'text-slate-500'
                        )}
                      />
                    </div>
                    <span className="flex-1 truncate">{project.name}</span>
                  </button>
                ))}
              </div>
            )}

            {/* Empty State */}
            {filteredProjects.length === 0 && !isLoading && (
              <div className="px-3 py-12 text-center">
                <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
                  <Folder className="h-6 w-6 text-slate-400" />
                </div>
                <p className="mb-1 text-sm font-medium text-slate-700">
                  {searchQuery ? 'No projects found' : 'No projects yet'}
                </p>
                <p className="mb-4 text-xs text-slate-500">
                  {searchQuery
                    ? 'Try adjusting your search'
                    : 'Create your first project to get started'}
                </p>
                {!searchQuery && canManage && (
                  <CreateProjectDialog
                    teamId={Number(teamId)}
                    open={isCreateDialogOpen}
                    onOpenChange={setIsCreateDialogOpen}
                  >
                    <Button
                      variant="outline"
                      size="sm"
                      className="gap-2 border-slate-300 text-slate-700 hover:bg-slate-50"
                    >
                      <Plus className="h-4 w-4" />
                      Create Project
                    </Button>
                  </CreateProjectDialog>
                )}
              </div>
            )}
          </div>
        )}
      </ScrollArea>

      {/* Footer - Quick Actions */}
      {canManage && !isLoading && projects.length > 0 && (
        <>
          <Separator />
          <div className="border-t border-slate-200 bg-slate-50 px-3 py-2">
            <CreateProjectDialog
              teamId={Number(teamId)}
              open={isCreateDialogOpen}
              onOpenChange={setIsCreateDialogOpen}
            >
              <Button
                variant="ghost"
                size="sm"
                className="w-full justify-start gap-2 text-slate-600 hover:bg-slate-100 hover:text-slate-900"
              >
                <Plus className="h-4 w-4" />
                Create New Project
              </Button>
            </CreateProjectDialog>
          </div>
        </>
      )}
    </div>
  );
}
