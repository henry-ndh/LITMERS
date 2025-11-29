import { useState } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Folder, Star } from 'lucide-react';
import ProjectList from './components/ProjectList';
import UpdateProjectDialog from './components/UpdateProjectDialog';
import {
  useGetMyProjects,
  useGetFavoriteProjects
} from '@/queries/project.query';
import __helpers from '@/helpers';

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

interface FavoriteProject {
  projectId: number;
  projectName: string;
  teamId: number;
  teamName: string;
  createdAt: string;
}

export default function ProjectPage() {
  const currentUserId = __helpers.getUserId();
  const [activeTab, setActiveTab] = useState('my-projects');
  const [projectToEdit, setProjectToEdit] = useState<Project | null>(null);

  const { data: myProjectsData, isLoading: isLoadingMyProjects } =
    useGetMyProjects();
  const { data: favoritesData, isLoading: isLoadingFavorites } =
    useGetFavoriteProjects();

  const myProjects: Project[] = myProjectsData?.data || myProjectsData || [];
  const favoritesRaw: FavoriteProject[] =
    favoritesData?.data || favoritesData || [];

  // Map favorite projects to Project format
  const favorites: Project[] = favoritesRaw.map((fav) => ({
    id: fav.projectId,
    teamId: fav.teamId,
    teamName: fav.teamName,
    ownerId: 0, // Not available in favorite response
    ownerName: '',
    ownerEmail: '',
    name: fav.projectName,
    description: null,
    isArchived: false, // Not available in favorite response
    isFavorite: true, // Always true for favorites
    createdAt: fav.createdAt,
    updatedAt: fav.createdAt // Use createdAt as fallback
  }));

  // Check if user can manage (is owner)
  // For favorite projects, ownerId is 0, so they can't be managed from favorites tab
  const canManageProject = (project: Project) => {
    if (!project.ownerId || project.ownerId === 0) return false;
    return project.ownerId?.toString() === currentUserId?.toString();
  };

  const handleEdit = (project: Project) => {
    setProjectToEdit(project);
  };

  return (
    <div className="container mx-auto max-w-7xl px-4 py-8">
      <div className="mb-8">
        <h1 className="mb-2 text-3xl font-bold tracking-tight">Projects</h1>
        <p className="text-muted-foreground">
          Manage and organize your projects
        </p>
      </div>

      <Tabs
        value={activeTab}
        onValueChange={setActiveTab}
        className="space-y-6"
      >
        <TabsList className="grid w-full grid-cols-2 lg:inline-grid lg:w-auto">
          <TabsTrigger value="my-projects" className="flex items-center gap-2">
            <Folder className="h-4 w-4" />
            <span className="hidden sm:inline">My Projects</span>
          </TabsTrigger>
          <TabsTrigger value="favorites" className="flex items-center gap-2">
            <Star className="h-4 w-4" />
            <span className="hidden sm:inline">Favorites</span>
          </TabsTrigger>
        </TabsList>

        <TabsContent value="my-projects" className="space-y-6">
          <ProjectList
            projects={myProjects}
            isLoading={isLoadingMyProjects}
            onEdit={handleEdit}
            canManage={canManageProject}
            showCreateButton={true}
            emptyMessage="No projects yet"
          />
        </TabsContent>

        <TabsContent value="favorites" className="space-y-6">
          <ProjectList
            projects={favorites}
            isLoading={isLoadingFavorites}
            onEdit={handleEdit}
            canManage={canManageProject}
            showCreateButton={false}
            emptyMessage="No favorite projects yet"
          />
        </TabsContent>
      </Tabs>

      <UpdateProjectDialog
        project={projectToEdit}
        open={!!projectToEdit}
        onOpenChange={(open) => !open && setProjectToEdit(null)}
      />
    </div>
  );
}
