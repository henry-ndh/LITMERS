import BaseRequest from '@/config/axios.config';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

// ISSUE STATUS MANAGEMENT

export const useGetIssueStatusesByProjectId = (projectId?: string | number) => {
  return useQuery({
    queryKey: ['issue-status', 'project', projectId],
    queryFn: async () => {
      if (!projectId) return [];
      return await BaseRequest.Get(`/api/issue/status/project/${projectId}`);
    },
    enabled: !!projectId
  });
};

export const useGetIssueStatusById = (statusId?: string | number) => {
  return useQuery({
    queryKey: ['issue-status', 'by-id', statusId],
    queryFn: async () => {
      if (!statusId) return null;
      return await BaseRequest.Get(`/api/issue/status/${statusId}`);
    },
    enabled: !!statusId
  });
};

export const useCreateIssueStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-status', 'create'],
    mutationFn: async (payload: {
      projectId: number;
      name: string;
      color?: string;
      position: number;
      isDefault?: boolean;
      wipLimit?: number;
    }) => {
      const { projectId, ...dto } = payload;
      return await BaseRequest.Post(
        `/api/issue/status?projectId=${projectId}`,
        dto
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue-status', 'project', variables.projectId]
      });
    }
  });
};

export const useUpdateIssueStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-status', 'update'],
    mutationFn: async (payload: {
      statusId: number;
      projectId: number;
      name?: string;
      color?: string;
      position?: number;
      isDefault?: boolean;
      wipLimit?: number;
    }) => {
      const { statusId, projectId, ...dto } = payload;
      return await BaseRequest.Put(
        `/api/issue/status/${statusId}?projectId=${projectId}`,
        dto
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue-status', 'project', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue-status', 'by-id', variables.statusId]
      });
    }
  });
};

export const useDeleteIssueStatus = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-status', 'delete'],
    mutationFn: async (payload: { statusId: number; projectId: number }) => {
      const { statusId, projectId } = payload;
      return await BaseRequest.Delete(
        `/api/issue/status/${statusId}?projectId=${projectId}`
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue-status', 'project', variables.projectId]
      });
    }
  });
};

export const useReorderStatuses = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-status', 'reorder'],
    mutationFn: async (payload: { projectId: number; statusIds: number[] }) => {
      const { projectId, statusIds } = payload;
      return await BaseRequest.Put(
        `/api/issue/status/reorder?projectId=${projectId}`,
        statusIds
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue-status', 'project', variables.projectId]
      });
    }
  });
};

// ISSUE MANAGEMENT

export const useGetIssuesByProjectId = (projectId?: string | number) => {
  return useQuery({
    queryKey: ['issue', 'project', projectId],
    queryFn: async () => {
      if (!projectId) return [];
      return await BaseRequest.Get(`/api/issue/project/${projectId}`);
    },
    enabled: !!projectId
  });
};

export const useGetIssuesByStatusId = (statusId?: string | number) => {
  return useQuery({
    queryKey: ['issue', 'status', statusId],
    queryFn: async () => {
      if (!statusId) return [];
      return await BaseRequest.Get(`/api/issue/status/${statusId}/issues`);
    },
    enabled: !!statusId
  });
};

export const useGetIssueById = (issueId?: string | number) => {
  return useQuery({
    queryKey: ['issue', 'by-id', issueId],
    queryFn: async () => {
      if (!issueId) return null;
      return await BaseRequest.Get(`/api/issue/${issueId}`);
    },
    enabled: !!issueId
  });
};

export const useGetIssueDetail = (issueId?: string | number) => {
  return useQuery({
    queryKey: ['issue', 'detail', issueId],
    queryFn: async () => {
      if (!issueId) return null;
      return await BaseRequest.Get(`/api/issue/${issueId}/detail`);
    },
    enabled: !!issueId
  });
};

export const useCreateIssue = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue', 'create'],
    mutationFn: async (dto: {
      projectId: number;
      statusId: number;
      title: string;
      description?: string;
      assigneeId?: number;
      dueDate?: string;
      priority?: number;
      position: number;
      labelIds?: number[];
    }) => {
      return await BaseRequest.Post(`/api/issue`, dto);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'project', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'status', variables.statusId]
      });
    }
  });
};

export const useUpdateIssue = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue', 'update'],
    mutationFn: async (payload: {
      issueId: number;
      statusId?: number;
      title?: string;
      description?: string;
      assigneeId?: number;
      dueDate?: string;
      priority?: number;
      position?: number;
      labelIds?: number[];
    }) => {
      const { issueId, ...dto } = payload;
      return await BaseRequest.Put(`/api/issue/${issueId}`, dto);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'project']
      });
    }
  });
};

export const useDeleteIssue = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue', 'delete'],
    mutationFn: async (issueId: number) => {
      return await BaseRequest.Delete(`/api/issue/${issueId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'project']
      });
    }
  });
};

// 👉 ĐÃ CHỈNH SỬA Ở ĐÂY
export const useMoveIssue = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue', 'move'],
    mutationFn: async (payload: {
      issueId: number;
      statusId: number;
      position: number;
      projectId: number;
    }) => {
      const { issueId, projectId, ...dto } = payload;
      // backend chỉ cần statusId + position
      return await BaseRequest.Put(`/api/issue/${issueId}/move`, dto);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'project', variables.projectId]
      });
    }
  });
};

// PROJECT LABEL MANAGEMENT

export const useGetProjectLabels = (projectId?: string | number) => {
  return useQuery({
    queryKey: ['label', 'project', projectId],
    queryFn: async () => {
      if (!projectId) return [];
      return await BaseRequest.Get(`/api/issue/label/project/${projectId}`);
    },
    enabled: !!projectId
  });
};

export const useGetProjectLabelById = (labelId?: string | number) => {
  return useQuery({
    queryKey: ['label', 'by-id', labelId],
    queryFn: async () => {
      if (!labelId) return null;
      return await BaseRequest.Get(`/api/issue/label/${labelId}`);
    },
    enabled: !!labelId
  });
};

export const useCreateProjectLabel = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['label', 'create'],
    mutationFn: async (payload: {
      projectId: number;
      name: string;
      color: string;
    }) => {
      const { projectId, ...dto } = payload;
      return await BaseRequest.Post(
        `/api/issue/label?projectId=${projectId}`,
        dto
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['label', 'project', variables.projectId]
      });
    }
  });
};

export const useUpdateProjectLabel = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['label', 'update'],
    mutationFn: async (payload: {
      labelId: number;
      projectId: number;
      name?: string;
      color?: string;
    }) => {
      const { labelId, projectId, ...dto } = payload;
      return await BaseRequest.Put(
        `/api/issue/label/${labelId}?projectId=${projectId}`,
        dto
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['label', 'project', variables.projectId]
      });
      queryClient.invalidateQueries({
        queryKey: ['label', 'by-id', variables.labelId]
      });
    }
  });
};

export const useDeleteProjectLabel = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['label', 'delete'],
    mutationFn: async (payload: { labelId: number; projectId: number }) => {
      const { labelId, projectId } = payload;
      return await BaseRequest.Delete(
        `/api/issue/label/${labelId}?projectId=${projectId}`
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['label', 'project', variables.projectId]
      });
    }
  });
};

// ISSUE LABEL MANAGEMENT

export const useAddLabelToIssue = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-label', 'add'],
    mutationFn: async (payload: { issueId: number; labelId: number }) => {
      const { issueId, labelId } = payload;
      return await BaseRequest.Post(
        `/api/issue/${issueId}/label/${labelId}`,
        {}
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
    }
  });
};

export const useRemoveLabelFromIssue = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-label', 'remove'],
    mutationFn: async (payload: { issueId: number; labelId: number }) => {
      const { issueId, labelId } = payload;
      return await BaseRequest.Delete(`/api/issue/${issueId}/label/${labelId}`);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
    }
  });
};

export const useUpdateIssueLabels = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['issue-label', 'update'],
    mutationFn: async (payload: { issueId: number; labelIds: number[] }) => {
      const { issueId, labelIds } = payload;
      return await BaseRequest.Put(`/api/issue/${issueId}/labels`, labelIds);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
    }
  });
};

// SUBTASK MANAGEMENT

export const useGetSubtasksByIssueId = (issueId?: string | number) => {
  return useQuery({
    queryKey: ['subtask', 'issue', issueId],
    queryFn: async () => {
      if (!issueId) return [];
      return await BaseRequest.Get(`/api/issue/${issueId}/subtasks`);
    },
    enabled: !!issueId
  });
};

export const useGetSubtaskById = (subtaskId?: string | number) => {
  return useQuery({
    queryKey: ['subtask', 'by-id', subtaskId],
    queryFn: async () => {
      if (!subtaskId) return null;
      return await BaseRequest.Get(`/api/issue/subtask/${subtaskId}`);
    },
    enabled: !!subtaskId
  });
};

export const useCreateSubtask = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['subtask', 'create'],
    mutationFn: async (payload: {
      issueId: number;
      title: string;
      position: number;
    }) => {
      const { issueId, ...dto } = payload;
      return await BaseRequest.Post(`/api/issue/${issueId}/subtask`, dto);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['subtask', 'issue', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
    }
  });
};

export const useUpdateSubtask = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['subtask', 'update'],
    mutationFn: async (payload: {
      subtaskId: number;
      issueId: number;
      title?: string;
      isDone?: boolean;
      position?: number;
      assigneeId?: number | null;
    }) => {
      const { subtaskId, issueId, ...dto } = payload;
      return await BaseRequest.Put(
        `/api/issue/subtask/${subtaskId}?issueId=${issueId}`,
        dto
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['subtask', 'issue', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['subtask', 'by-id', variables.subtaskId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
    }
  });
};

export const useDeleteSubtask = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['subtask', 'delete'],
    mutationFn: async (payload: { subtaskId: number; issueId: number }) => {
      const { subtaskId, issueId } = payload;
      return await BaseRequest.Delete(
        `/api/issue/subtask/${subtaskId}?issueId=${issueId}`
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['subtask', 'issue', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'detail', variables.issueId]
      });
      queryClient.invalidateQueries({
        queryKey: ['issue', 'by-id', variables.issueId]
      });
    }
  });
};

export const useReorderSubtasks = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['subtask', 'reorder'],
    mutationFn: async (payload: { issueId: number; subtaskIds: number[] }) => {
      const { issueId, subtaskIds } = payload;
      return await BaseRequest.Put(
        `/api/issue/${issueId}/subtasks/reorder`,
        subtaskIds
      );
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['subtask', 'issue', variables.issueId]
      });
    }
  });
};

// ISSUE HISTORY

export const useGetIssueHistory = (
  issueId?: string | number,
  limit: number = 50
) => {
  return useQuery({
    queryKey: ['issue-history', issueId, limit],
    queryFn: async () => {
      if (!issueId) return [];
      return await BaseRequest.Get(
        `/api/issue/${issueId}/history?limit=${limit}`
      );
    },
    enabled: !!issueId
  });
};

// AI FEATURES

export const useGetAISummary = () => {
  return useMutation({
    mutationKey: ['ai', 'summary'],
    mutationFn: async (issueId: number) => {
      return await BaseRequest.Post(`/api/issue/${issueId}/ai/summary`);
    }
  });
};

export const useGetAISuggestion = () => {
  return useMutation({
    mutationKey: ['ai', 'suggestion'],
    mutationFn: async (issueId: number) => {
      return await BaseRequest.Post(`/api/issue/${issueId}/ai/suggestion`);
    }
  });
};
