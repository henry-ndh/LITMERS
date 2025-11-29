import { useState } from 'react';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Search, FolderPlus } from 'lucide-react';
import ProjectCard from './ProjectCard';
import CreateProjectDialog from './CreateProjectDialog';

interface Project {
  id: number;
  teamId: number;
  teamName: string;
  ownerId: number;
  ownerName: string;
  ownerEmail: string;
  name: string;
  description?: string | null;
  isArchived: boolean;
  isFavorite: boolean;
  createdAt: string;
  updatedAt: string;
}

interface ProjectListProps {
  projects: Project[];
  isLoading?: boolean;
  onEdit?: (project: Project) => void;
  canManage?: (project: Project) => boolean;
  teamId?: number;
  showCreateButton?: boolean;
  emptyMessage?: string;
}

export default function ProjectList({
  projects,
  isLoading = false,
  onEdit,
  canManage,
  teamId,
  showCreateButton = false,
  emptyMessage = 'No projects found'
}: ProjectListProps) {
  const defaultCanManage = () => true;
  const checkCanManage = canManage || defaultCanManage;
  const [searchQuery, setSearchQuery] = useState('');
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);

  const filteredProjects = projects.filter(
    (project) =>
      project?.name?.toLowerCase().includes(searchQuery.toLowerCase()) ||
      project?.description?.toLowerCase().includes(searchQuery.toLowerCase()) ||
      project?.teamName?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const activeProjects = filteredProjects.filter((p) => !p.isArchived);
  const archivedProjects = filteredProjects.filter((p) => p.isArchived);

  if (isLoading) {
    return (
      <Card>
        <CardContent className="p-12">
          <div className="text-center text-muted-foreground">Loading...</div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Search and Create */}
      <div className="flex items-center gap-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 transform text-muted-foreground" />
          <Input
            placeholder="Search projects..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="pl-10"
          />
        </div>
        {showCreateButton && (
          <CreateProjectDialog
            teamId={teamId}
            open={isCreateDialogOpen}
            onOpenChange={setIsCreateDialogOpen}
          >
            <button className="inline-flex items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90">
              <FolderPlus className="h-4 w-4" />
              Create Project
            </button>
          </CreateProjectDialog>
        )}
      </div>

      {/* Active Projects */}
      {activeProjects.length > 0 && (
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-muted-foreground">
            Active Projects
          </h3>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
            {activeProjects.map((project) => (
              <ProjectCard
                key={project.id}
                project={project}
                onEdit={onEdit}
                canManage={checkCanManage}
              />
            ))}
          </div>
        </div>
      )}

      {/* Archived Projects */}
      {archivedProjects.length > 0 && (
        <div className="space-y-4">
          <h3 className="text-sm font-medium text-muted-foreground">
            Archived Projects
          </h3>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
            {archivedProjects.map((project) => (
              <ProjectCard
                key={project.id}
                project={project}
                onEdit={onEdit}
                canManage={checkCanManage}
              />
            ))}
          </div>
        </div>
      )}

      {/* Empty State */}
      {filteredProjects.length === 0 && (
        <Card>
          <CardContent className="p-12">
            <div className="text-center">
              <FolderPlus className="mx-auto mb-4 h-16 w-16 text-muted-foreground" />
              <h3 className="mb-2 text-lg font-semibold">
                {searchQuery ? 'No projects found' : emptyMessage}
              </h3>
              <p className="mb-6 text-muted-foreground">
                {searchQuery
                  ? 'Try searching with different keywords'
                  : showCreateButton
                    ? 'Create your first project to get started'
                    : 'No projects available'}
              </p>
              {!searchQuery && showCreateButton && (
                <CreateProjectDialog
                  teamId={teamId}
                  open={isCreateDialogOpen}
                  onOpenChange={setIsCreateDialogOpen}
                >
                  <button className="inline-flex items-center justify-center gap-2 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary/90">
                    <FolderPlus className="h-4 w-4" />
                    Create Your First Project
                  </button>
                </CreateProjectDialog>
              )}
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
