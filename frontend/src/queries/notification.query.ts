import BaseRequest from '@/config/axios.config';
import {
  useMutation,
  useQuery,
  useQueryClient,
  useInfiniteQuery
} from '@tanstack/react-query';

// NOTIFICATION MANAGEMENT

export const useGetNotifications = (limit: number = 20, isRead?: boolean) => {
  const params = new URLSearchParams();
  params.append('limit', limit.toString());
  if (isRead !== undefined) {
    params.append('isRead', isRead.toString());
  }

  return useQuery({
    queryKey: ['notification', 'list', limit, isRead],
    queryFn: async () => {
      const response = await BaseRequest.Get(`/api/notification?${params.toString()}`);
      return response?.data || response;
    }
  });
};

export const useGetNotificationsInfinite = (limit: number = 20) => {
  return useInfiniteQuery({
    queryKey: ['notification', 'infinite'],
    queryFn: async ({ pageParam = 0 }) => {
      const params = new URLSearchParams();
      // Calculate the limit for this page (increase limit each time)
      const currentLimit = limit * (pageParam + 1);
      params.append('limit', currentLimit.toString());

      const response = await BaseRequest.Get(`/api/notification?${params.toString()}`);
      const data = response?.data || response;
      const notifications = data?.notifications || data?.Notifications || [];
      
      return {
        ...data,
        notifications,
        // If we got less than requested, we've reached the end
        hasMore: notifications.length === currentLimit,
        nextPage: notifications.length === currentLimit ? pageParam + 1 : undefined
      };
    },
    getNextPageParam: (lastPage) => lastPage.nextPage,
    initialPageParam: 0
  });
};

export const useGetNotificationById = (notificationId?: string | number) => {
  return useQuery({
    queryKey: ['notification', 'by-id', notificationId],
    queryFn: async () => {
      if (!notificationId) return null;
      return await BaseRequest.Get(`/api/notification/${notificationId}`);
    },
    enabled: !!notificationId
  });
};

export const useGetUnreadCount = () => {
  return useQuery({
    queryKey: ['notification', 'unread-count'],
    queryFn: async () => {
      const response = await BaseRequest.Get(`/api/notification/unread-count`);
      return response?.data?.unreadCount || response?.unreadCount || 0;
    },
    refetchInterval: 30000 // Refetch every 30 seconds
  });
};

export const useMarkAsRead = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['notification', 'mark-read'],
    mutationFn: async (notificationId: number) => {
      return await BaseRequest.Put(`/api/notification/${notificationId}/read`, {});
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['notification', 'list']
      });
      queryClient.invalidateQueries({
        queryKey: ['notification', 'infinite']
      });
      queryClient.invalidateQueries({
        queryKey: ['notification', 'unread-count']
      });
    }
  });
};

export const useMarkAllAsRead = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['notification', 'mark-all-read'],
    mutationFn: async () => {
      return await BaseRequest.Put(`/api/notification/mark-all-read`, {});
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['notification', 'list']
      });
      queryClient.invalidateQueries({
        queryKey: ['notification', 'infinite']
      });
      queryClient.invalidateQueries({
        queryKey: ['notification', 'unread-count']
      });
    }
  });
};

export const useDeleteNotification = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationKey: ['notification', 'delete'],
    mutationFn: async (notificationId: number) => {
      return await BaseRequest.Delete(`/api/notification/${notificationId}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['notification', 'list']
      });
      queryClient.invalidateQueries({
        queryKey: ['notification', 'infinite']
      });
      queryClient.invalidateQueries({
        queryKey: ['notification', 'unread-count']
      });
    }
  });
};

