import { useParams, useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
  ArrowLeft,
  Folder,
  Star,
  Archive,
  ArchiveRestore,
  Edit,
  Trash2,
  Users,
  Calendar,
  User
} from 'lucide-react';
import {
  useGetProjectDetail,
  useArchiveProject,
  useDeleteProject,
  useAddFavoriteProject,
  useRemoveFavoriteProject
} from '@/queries/project.query';
import UpdateProjectDialog from './components/UpdateProjectDialog';
import KanbanBoard from '@/pages/IssuePage/components/KanbanBoard';
import { toast } from 'sonner';
import { useState } from 'react';
import __helpers from '@/helpers';

export default function ProjectDetailPage() {
  const { projectId } = useParams<{ projectId: string }>();
  const navigate = useNavigate();
  const currentUserId = __helpers.getUserId();
  const [activeTab, setActiveTab] = useState('overview');
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false);

  const { data: projectData, isLoading } = useGetProjectDetail(projectId);
  const archiveMutation = useArchiveProject();
  const deleteMutation = useDeleteProject();
  const addFavoriteMutation = useAddFavoriteProject();
  const removeFavoriteMutation = useRemoveFavoriteProject();

  const project = projectData?.data || projectData;

  const canManage = project?.ownerId?.toString() === currentUserId?.toString();

  const handleArchive = async () => {
    if (!project) return;

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
    if (!project) return;

    if (
      !confirm(
        `Are you sure you want to delete "${project.name}"? This action cannot be undone.`
      )
    ) {
      return;
    }

    try {
      await deleteMutation.mutateAsync(project.id);
      toast.success('Project deleted successfully');
      navigate('/project');
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to delete project');
    }
  };

  const handleToggleFavorite = async () => {
    if (!project) return;

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

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <div className="text-muted-foreground">Loading...</div>
      </div>
    );
  }

  if (!project) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <div className="space-y-4 text-center">
          <div className="text-muted-foreground">Project not found</div>
          <Button onClick={() => navigate('/project')} variant="outline">
            Back to Projects
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Navigation breadcrumb */}
      <div className="border-b">
        <div className="container mx-auto max-w-7xl px-4 py-3">
          <Button
            variant="ghost"
            onClick={() => navigate('/project')}
            className="gap-2"
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Projects
          </Button>
        </div>
      </div>

      <div className="container mx-auto max-w-7xl px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-start justify-between gap-4">
            <div className="flex-1">
              <div className="mb-2 flex items-center gap-3">
                <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                  <Folder className="h-6 w-6 text-primary" />
                </div>
                <div className="flex-1">
                  <h1 className="text-3xl font-bold tracking-tight">
                    {project.name}
                  </h1>
                  {project.teamName && (
                    <div className="mt-1 flex items-center gap-1">
                      <Users className="h-4 w-4 text-muted-foreground" />
                      <span className="text-sm text-muted-foreground">
                        {project.teamName}
                      </span>
                    </div>
                  )}
                </div>
              </div>

              {project.description && (
                <p className="mt-4 max-w-3xl text-muted-foreground">
                  {project.description}
                </p>
              )}

              <div className="mt-4 flex flex-wrap items-center gap-2">
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
            </div>

            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={handleToggleFavorite}
              >
                <Star
                  className={`mr-2 h-4 w-4 ${
                    project.isFavorite
                      ? 'fill-yellow-400 text-yellow-400'
                      : 'text-muted-foreground'
                  }`}
                />
                {project.isFavorite ? 'Unfavorite' : 'Favorite'}
              </Button>

              {canManage && (
                <>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setIsEditDialogOpen(true)}
                  >
                    <Edit className="mr-2 h-4 w-4" />
                    Edit
                  </Button>
                  <Button variant="outline" size="sm" onClick={handleArchive}>
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
                  </Button>
                  <Button
                    variant="destructive"
                    size="sm"
                    onClick={handleDelete}
                  >
                    <Trash2 className="mr-2 h-4 w-4" />
                    Delete
                  </Button>
                </>
              )}
            </div>
          </div>
        </div>

        {/* Tabs */}
        <Tabs
          value={activeTab}
          onValueChange={setActiveTab}
          className="space-y-6"
        >
          <TabsList>
            <TabsTrigger value="overview">Overview</TabsTrigger>
            <TabsTrigger value="issues">Issues</TabsTrigger>
          </TabsList>

          <TabsContent value="overview" className="space-y-6">
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              {/* Project Info */}
              <Card>
                <CardHeader>
                  <CardTitle>Project Information</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Owner
                    </p>
                    <div className="mt-1 flex items-center gap-2">
                      <User className="h-4 w-4 text-muted-foreground" />
                      <span>{project.ownerName || 'N/A'}</span>
                    </div>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Team
                    </p>
                    <div className="mt-1 flex items-center gap-2">
                      <Users className="h-4 w-4 text-muted-foreground" />
                      <span>{project.teamName || 'N/A'}</span>
                    </div>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Created
                    </p>
                    <div className="mt-1 flex items-center gap-2">
                      <Calendar className="h-4 w-4 text-muted-foreground" />
                      <span>
                        {new Date(project.createdAt).toLocaleDateString(
                          'en-US',
                          {
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric'
                          }
                        )}
                      </span>
                    </div>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Last Updated
                    </p>
                    <div className="mt-1 flex items-center gap-2">
                      <Calendar className="h-4 w-4 text-muted-foreground" />
                      <span>
                        {new Date(project.updatedAt).toLocaleDateString(
                          'en-US',
                          {
                            year: 'numeric',
                            month: 'long',
                            day: 'numeric'
                          }
                        )}
                      </span>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Statistics */}
              <Card>
                <CardHeader>
                  <CardTitle>Statistics</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Total Issues
                    </p>
                    <p className="mt-1 text-2xl font-bold">
                      {project.issueCount || 0}
                    </p>
                  </div>
                </CardContent>
              </Card>
            </div>
          </TabsContent>

          <TabsContent value="issues" className="space-y-6">
            <KanbanBoard projectId={projectId || ''} canManage={canManage} />
          </TabsContent>
        </Tabs>
      </div>

      <UpdateProjectDialog
        project={project}
        open={isEditDialogOpen}
        onOpenChange={setIsEditDialogOpen}
      />
    </div>
  );
}
