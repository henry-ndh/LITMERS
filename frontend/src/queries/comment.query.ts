import BaseRequest from '@/config/axios.config';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

// COMMENT MANAGEMENT

export const useGetCommentsByIssueId = (issueId?: string | number) => {
  return useQuery({
    queryKey: ['comment', 'issue', issueId],
    queryFn: async () => {
      if (!issueId) return [];
      return await BaseRequest.Get(`/api/comment/issue/${issueId}`);
    },
    enabled: !!issueId
  });
};

export const useGetCommentById = (commentId?: string | number) => {
  return useQuery({
    queryKey: ['comment', 'by-id', commentId],
    queryFn: async () => {
      if (!commentId) return null;
      return await BaseRequest.Get(`/api/comment/${commentId}`);
    },
    enabled: !!commentId
  });
};

export const useCreateComment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['comment', 'create'],
    mutationFn: async (dto: { issueId: number; content: string }) => {
      return await BaseRequest.Post(`/api/comment`, dto);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['comment', 'issue', variables.issueId]
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

export const useUpdateComment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['comment', 'update'],
    mutationFn: async (payload: { commentId: number; content: string }) => {
      const { commentId, ...dto } = payload;
      return await BaseRequest.Put(`/api/comment/${commentId}`, dto);
    },
    onSuccess: (_data, variables) => {
      queryClient.invalidateQueries({
        queryKey: ['comment', 'by-id', variables.commentId]
      });
      queryClient.invalidateQueries({
        queryKey: ['comment', 'issue']
      });
    }
  });
};

export const useDeleteComment = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['comment', 'delete'],
    mutationFn: async (commentId: number) => {
      return await BaseRequest.Delete(`/api/comment/${commentId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['comment', 'issue']
      });
    }
  });
};

