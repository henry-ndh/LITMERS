import { useState } from 'react';
import ProjectList from '@/pages/ProjectPage/components/ProjectList';
import UpdateProjectDialog from '@/pages/ProjectPage/components/UpdateProjectDialog';
import { useGetProjectsByTeamId } from '@/queries/project.query';
import __helpers from '@/helpers';

interface TeamProjectsProps {
  teamId: string;
  currentUserId: string;
}

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

export default function TeamProjects({ teamId, currentUserId }: TeamProjectsProps) {
  const [projectToEdit, setProjectToEdit] = useState<Project | null>(null);

  const { data: projectsData, isLoading } = useGetProjectsByTeamId(teamId);

  const projects: Project[] = projectsData?.data || projectsData || [];

  // Check if user can manage (is owner of project)
  const canManageProject = (project: Project) => {
    return project.ownerId?.toString() === currentUserId?.toString();
  };

  const handleEdit = (project: Project) => {
    setProjectToEdit(project);
  };

  return (
    <>
      <ProjectList
        projects={projects}
        isLoading={isLoading}
        onEdit={handleEdit}
        canManage={canManageProject}
        teamId={parseInt(teamId)}
        showCreateButton={true}
        emptyMessage="No projects in this team yet"
      />

      <UpdateProjectDialog
        project={projectToEdit}
        open={!!projectToEdit}
        onOpenChange={(open) => !open && setProjectToEdit(null)}
      />
    </>
  );
}

