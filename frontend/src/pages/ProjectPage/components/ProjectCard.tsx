import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from '@/components/ui/dropdown-menu';
import {
  Folder,
  Star,
  Archive,
  ArchiveRestore,
  MoreVertical,
  Trash2,
  Edit,
  Users
} from 'lucide-react';
import {
  useArchiveProject,
  useDeleteProject,
  useAddFavoriteProject,
  useRemoveFavoriteProject
} from '@/queries/project.query';
import { toast } from 'sonner';

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

interface ProjectCardProps {
  project: Project;
  onEdit?: (project: Project) => void;
  canManage?: (project: Project) => boolean;
}

export default function ProjectCard({
  project,
  onEdit,
  canManage
}: ProjectCardProps) {
  const navigate = useNavigate();
  const [isDeleting, setIsDeleting] = useState(false);
  const userCanManage = canManage ? canManage(project) : false;
  const archiveMutation = useArchiveProject();
  const deleteMutation = useDeleteProject();
  const addFavoriteMutation = useAddFavoriteProject();
  const removeFavoriteMutation = useRemoveFavoriteProject();

  const handleArchive = async () => {
    // Don't allow archive if project info is incomplete (e.g., from favorites)
    if (project.ownerId === 0) {
      toast.error(
        'Cannot archive project from favorites. Please view project details first.'
      );
      return;
    }

    try {
      await archiveMutation.mutateAsync({
        projectId: project.id,
        isArchived: !project.isArchived
      });
      toast.success(
        project.isArchived
          ? 'Project unarchived successfully'
          : 'Project archived successfully'
      );
    } catch (error: any) {
      toast.error(
        error?.response?.data?.message ||
          'Failed to update project archive status'
      );
    }
  };

  const handleDelete = async () => {
    if (isDeleting) return;

    if (
      !confirm(
        `Are you sure you want to delete "${project.name}"? This action cannot be undone.`
      )
    ) {
      return;
    }

    setIsDeleting(true);
    try {
      await deleteMutation.mutateAsync(project.id);
      toast.success('Project deleted successfully');
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to delete project');
      setIsDeleting(false);
    }
  };

  const handleToggleFavorite = async () => {
    try {
      if (project.isFavorite) {
        await removeFavoriteMutation.mutateAsync(project.id);
        toast.success('Removed from favorites');
      } else {
        await addFavoriteMutation.mutateAsync(project.id);
        toast.success('Added to favorites');
      }
    } catch (error: any) {
      toast.error(
        error?.response?.data?.message || 'Failed to update favorite status'
      );
    }
  };

  return (
    <Card
      className={`group cursor-pointer transition-all hover:shadow-md ${
        project.isArchived ? 'opacity-60' : ''
      }`}
      onClick={() => navigate(`/project/${project.id}`)}
    >
      <CardHeader className="space-y-3">
        <div className="flex items-start justify-between">
          <div className="flex min-w-0 flex-1 items-center gap-3">
            <div className="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-primary/10">
              <Folder className="h-5 w-5 text-primary" />
            </div>
            <div className="min-w-0 flex-1">
              <h3 className="truncate text-lg font-semibold transition-colors group-hover:text-primary">
                {project.name}
              </h3>
              {project.teamName && (
                <div className="mt-1 flex items-center gap-1">
                  <Users className="h-3 w-3 text-muted-foreground" />
                  <span className="truncate text-xs text-muted-foreground">
                    {project.teamName}
                  </span>
                </div>
              )}
            </div>
          </div>

          <div
            className="flex flex-shrink-0 items-center gap-2"
            onClick={(e) => e.stopPropagation()}
          >
            <Button
              variant="ghost"
              size="sm"
              className="h-8 w-8 p-0"
              onClick={handleToggleFavorite}
            >
              <Star
                className={`h-4 w-4 ${
                  project.isFavorite
                    ? 'fill-yellow-400 text-yellow-400'
                    : 'text-muted-foreground'
                }`}
              />
            </Button>

            {userCanManage && project.ownerId !== 0 && (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button
                    variant="ghost"
                    size="sm"
                    className="h-8 w-8 p-0"
                    onClick={(e) => e.stopPropagation()}
                  >
                    <MoreVertical className="h-4 w-4" />
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  {onEdit && (
                    <DropdownMenuItem onClick={() => onEdit(project)}>
                      <Edit className="mr-2 h-4 w-4" />
                      Edit
                    </DropdownMenuItem>
                  )}
                  <DropdownMenuItem onClick={handleArchive}>
                    {project.isArchived ? (
                      <>
                        <ArchiveRestore className="mr-2 h-4 w-4" />
                        Unarchive
                      </>
                    ) : (
                      <>
                        <Archive className="mr-2 h-4 w-4" />
                        Archive
                      </>
                    )}
                  </DropdownMenuItem>
                  <DropdownMenuSeparator />
                  <DropdownMenuItem
                    onClick={handleDelete}
                    className="text-destructive focus:text-destructive"
                  >
                    <Trash2 className="mr-2 h-4 w-4" />
                    Delete
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            )}
          </div>
        </div>

        <div className="flex flex-wrap items-center gap-2">
          {project.isArchived && (
            <Badge variant="outline" className="bg-gray-50">
              <Archive className="mr-1 h-3 w-3" />
              Archived
            </Badge>
          )}
          {project.isFavorite && (
            <Badge
              variant="outline"
              className="border-yellow-200 bg-yellow-50 text-yellow-700"
            >
              <Star className="mr-1 h-3 w-3 fill-yellow-400 text-yellow-400" />
              Favorite
            </Badge>
          )}
        </div>
      </CardHeader>

      {project.description && (
        <CardContent>
          <p className="line-clamp-2 text-sm text-muted-foreground">
            {project.description}
          </p>
        </CardContent>
      )}
    </Card>
  );
}
