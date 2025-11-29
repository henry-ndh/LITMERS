import BaseRequest from '@/config/axios.config';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

// TEAM MANAGEMENT

export const useGetMyTeams = () => {
  return useQuery({
    queryKey: ['team', 'my-teams'],
    queryFn: async () => {
      return await BaseRequest.Get(`/api/team/my-teams`);
    }
  });
};

export const useGetTeamById = (teamId?: string | number) => {
  return useQuery({
    queryKey: ['team', 'by-id', teamId],
    queryFn: async () => {
      if (!teamId) return null;
      return await BaseRequest.Get(`/api/team/${teamId}`);
    },
    enabled: !!teamId
  });
};

export const useGetTeamDetail = (teamId?: string | number) => {
  return useQuery({
    queryKey: ['team', 'detail', teamId],
    queryFn: async () => {
      if (!teamId) return null;
      return await BaseRequest.Get(`/api/team/${teamId}/detail`);
    },
    enabled: !!teamId
  });
};

export const useCreateTeam = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'create'],
    mutationFn: async (model: { name: string }) => {
      return await BaseRequest.Post(`/api/team`, model);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'my-teams']
      });
    }
  });
};

export const useUpdateTeam = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'update'],
    mutationFn: async (model: { id: string | number; name: string }) => {
      return await BaseRequest.Put(`/api/team/${model.id}`, { name: model.name });
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'my-teams']
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', variables.id]
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'by-id', variables.id]
      });
    }
  });
};

export const useDeleteTeam = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'delete'],
    mutationFn: async (teamId: string | number) => {
      return await BaseRequest.Delete(`/api/team/${teamId}`);
    },
    onSuccess: (_data, teamId) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'my-teams']
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', teamId]
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'by-id', teamId]
      });
    }
  });
};

// TEAM MEMBERS

export const useGetTeamMembers = (teamId?: string | number) => {
  return useQuery({
    queryKey: ['team', 'members', teamId],
    queryFn: async () => {
      if (!teamId) return [];
      return await BaseRequest.Get(`/api/team/${teamId}/members`);
    },
    enabled: !!teamId
  });
};

export const useUpdateMemberRole = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'member', 'update-role'],
    mutationFn: async (payload: {
      teamId: string | number;
      memberId: string | number;
      role: number;
    }) => {
      const { teamId, memberId, role } = payload;
      return await BaseRequest.Put(
        `/api/team/${teamId}/members/${memberId}/role`,
        { role }
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'members', variables.teamId]
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', variables.teamId]
      });
    }
  });
};

export const useRemoveMember = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'member', 'remove'],
    mutationFn: async (payload: { teamId: string | number; memberId: string | number }) => {
      const { teamId, memberId } = payload;
      return await BaseRequest.Delete(
        `/api/team/${teamId}/members/${memberId}`
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'members', variables.teamId]
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', variables.teamId]
      });
    }
  });
};

export const useLeaveTeam = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'leave'],
    mutationFn: async (teamId: string | number) => {
      return await BaseRequest.Post(`/api/team/${teamId}/leave`, {});
    },
    onSuccess: (_data, teamId) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'my-teams']
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'members', teamId]
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', teamId]
      });
    }
  });
};

// TEAM INVITES

export const useInviteMember = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'invite', 'create'],
    mutationFn: async (payload: { teamId: string | number; email: string }) => {
      const { teamId, email } = payload;
      return await BaseRequest.Post(`/api/team/${teamId}/invite`, { email });
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', variables.teamId]
      });
    }
  });
};

export const useCancelInvite = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'invite', 'cancel'],
    mutationFn: async (payload: {
      teamId: string | number;
      inviteId: string | number;
    }) => {
      const { teamId, inviteId } = payload;
      return await BaseRequest.Delete(
        `/api/team/${teamId}/invites/${inviteId}`
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'detail', variables.teamId]
      });
    }
  });
};

export const useGetMyInvites = () => {
  return useQuery({
    queryKey: ['team', 'my-invites'],
    queryFn: async () => {
      return await BaseRequest.Get(`/api/team/my-invites`);
    }
  });
};

export const useAcceptInvite = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['team', 'invite', 'accept'],
    mutationFn: async (payload: { token: string }) => {
      return await BaseRequest.Post(`/api/team/accept-invite`, payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['team', 'my-invites']
      });
      queryClient.invalidateQueries({
        queryKey: ['team', 'my-teams']
      });
    }
  });
};

// ACTIVITY LOGS

export const useGetActivityLogs = (teamId?: string | number, limit = 50) => {
  return useQuery({
    queryKey: ['team', 'activity-logs', teamId, limit],
    queryFn: async () => {
      if (!teamId) return [];
      return await BaseRequest.Get(
        `/api/team/${teamId}/activity-logs?limit=${limit}`
      );
    },
    enabled: !!teamId
  });
};


