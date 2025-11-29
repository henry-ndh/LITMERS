import BaseRequest from '@/config/axios.config';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

// PROJECT MANAGEMENT

export const useGetMyProjects = () => {
  return useQuery({
    queryKey: ['project', 'my-projects'],
    queryFn: async () => {
      return await BaseRequest.Get(`/api/project/my-projects`);
    }
  });
};

export const useGetProjectsByTeamId = (teamId?: string | number) => {
  return useQuery({
    queryKey: ['project', 'team', teamId],
    queryFn: async () => {
      if (!teamId) return [];
      return await BaseRequest.Get(`/api/project/team/${teamId}`);
    },
    enabled: !!teamId
  });
};

export const useGetProjectById = (projectId?: string | number) => {
  return useQuery({
    queryKey: ['project', 'by-id', projectId],
    queryFn: async () => {
      if (!projectId) return null;
      return await BaseRequest.Get(`/api/project/${projectId}`);
    },
    enabled: !!projectId
  });
};

export const useGetProjectDetail = (projectId?: string | number) => {
  return useQuery({
    queryKey: ['project', 'detail', projectId],
    queryFn: async () => {
      if (!projectId) return null;
      return await BaseRequest.Get(`/api/project/${projectId}/detail`);
    },
    enabled: !!projectId
  });
};

export const useGetFavoriteProjects = () => {
  return useQuery({
    queryKey: ['project', 'favorites'],
    queryFn: async () => {
      return await BaseRequest.Get(`/api/project/favorites`);
    }
  });
};

export const useCreateProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['project', 'create'],
    mutationFn: async (model: { teamId: number; name: string; description?: string }) => {
      return await BaseRequest.Post(`/api/project`, model);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['project', 'my-projects']
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'team', variables.teamId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'favorites']
      });
    }
  });
};

export const useUpdateProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['project', 'update'],
    mutationFn: async (payload: {
      projectId: string | number;
      name: string;
      description?: string;
    }) => {
      const { projectId, name, description } = payload;
      return await BaseRequest.Put(`/api/project/${projectId}`, { name, description });
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['project', 'by-id', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'detail', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'my-projects']
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'favorites']
      });
    }
  });
};

export const useDeleteProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['project', 'delete'],
    mutationFn: async (projectId: string | number) => {
      return await BaseRequest.Delete(`/api/project/${projectId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['project', 'my-projects']
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'favorites']
      });
    }
  });
};

export const useArchiveProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['project', 'archive'],
    mutationFn: async (payload: {
      projectId: string | number;
      isArchived: boolean;
    }) => {
      const { projectId, isArchived } = payload;
      return await BaseRequest.Put(`/api/project/${projectId}/archive`, { isArchived });
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['project', 'by-id', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'detail', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'my-projects']
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'favorites']
      });
    }
  });
};

// FAVORITE PROJECTS

export const useAddFavoriteProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['project', 'favorite', 'add'],
    mutationFn: async (projectId: string | number) => {
      return await BaseRequest.Post(`/api/project/${projectId}/favorite`, {});
    },
    onSuccess: (_data, projectId) => {
      queryClient.invalidateQueries({
        queryKey: ['project', 'favorites']
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'by-id', projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'detail', projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'my-projects']
      });
    }
  });
};

export const useRemoveFavoriteProject = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['project', 'favorite', 'remove'],
    mutationFn: async (projectId: string | number) => {
      return await BaseRequest.Delete(`/api/project/${projectId}/favorite`);
    },
    onSuccess: (_data, projectId) => {
      queryClient.invalidateQueries({
        queryKey: ['project', 'favorites']
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'by-id', projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'detail', projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['project', 'my-projects']
      });
    }
  });
};

