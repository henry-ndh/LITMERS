import { useState, useRef, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Popover,
  PopoverContent,
  PopoverTrigger
} from '@/components/ui/popover';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Bell, Check, CheckCheck, Trash2, Loader2 } from 'lucide-react';
import {
  useGetNotificationsInfinite,
  useGetUnreadCount,
  useMarkAsRead,
  useMarkAllAsRead,
  useDeleteNotification
} from '@/queries/notification.query';
import { useToast } from '@/components/ui/use-toast';
import { formatDistanceToNow } from 'date-fns';

export default function NotificationBell() {
  const { toast } = useToast();
  const [isOpen, setIsOpen] = useState(false);
  const viewportRef = useRef<HTMLDivElement>(null);

  const {
    data: notificationsData,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading
  } = useGetNotificationsInfinite(20);
  const { data: unreadCount } = useGetUnreadCount();
  const markAsReadMutation = useMarkAsRead();
  const markAllAsReadMutation = useMarkAllAsRead();
  const deleteNotificationMutation = useDeleteNotification();

  // Flatten all pages into a single array
  const notifications =
    notificationsData?.pages
      ?.flatMap((page: any) => page?.notifications || page?.Notifications || [])
      .filter(Boolean) || [];

  const unreadNotifications = Array.isArray(notifications)
    ? notifications.filter((n: any) => !n.isRead)
    : [];

  // Handle scroll for infinite loading
  useEffect(() => {
    if (!isOpen) return;

    // Find the viewport element inside ScrollArea
    const findViewport = () => {
      const scrollArea = viewportRef.current?.closest(
        '[data-radix-scroll-area-viewport]'
      );
      return scrollArea as HTMLElement | null;
    };

    const viewport = findViewport();
    if (!viewport) return;

    const handleScroll = () => {
      const { scrollTop, scrollHeight, clientHeight } = viewport;
      // Load more when scrolled to bottom (50px threshold)
      if (
        scrollTop + clientHeight >= scrollHeight - 50 &&
        hasNextPage &&
        !isFetchingNextPage
      ) {
        fetchNextPage();
      }
    };

    viewport.addEventListener('scroll', handleScroll);
    return () => viewport.removeEventListener('scroll', handleScroll);
  }, [isOpen, hasNextPage, isFetchingNextPage, fetchNextPage]);

  const handleMarkAsRead = async (notificationId: number) => {
    try {
      await markAsReadMutation.mutateAsync(notificationId);
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Notification marked as read'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message ||
          'Failed to mark notification as read',
        variant: 'destructive'
      });
    }
  };

  const handleMarkAllAsRead = async () => {
    try {
      await markAllAsReadMutation.mutateAsync();
      toast({
        title: 'Success',
        variant: 'success',
        description: 'All notifications marked as read'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to mark all as read',
        variant: 'destructive'
      });
    }
  };

  const handleDelete = async (notificationId: number) => {
    try {
      await deleteNotificationMutation.mutateAsync(notificationId);
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Notification deleted'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to delete notification',
        variant: 'destructive'
      });
    }
  };

  return (
    <Popover open={isOpen} onOpenChange={setIsOpen}>
      <PopoverTrigger asChild>
        <Button variant="ghost" size="sm" className="relative">
          <Bell className="h-5 w-5" />
          {unreadCount && unreadCount > 0 && (
            <Badge
              variant="destructive"
              className="absolute -right-1 -top-1 flex h-5 w-5 items-center justify-center p-0 text-xs"
            >
              {unreadCount > 99 ? '99+' : unreadCount}
            </Badge>
          )}
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-80 p-0" align="end">
        <div className="flex items-center justify-between border-b px-4 py-3">
          <h3 className="font-semibold">Notifications</h3>
          {unreadNotifications.length > 0 && (
            <Button
              variant="ghost"
              size="sm"
              onClick={handleMarkAllAsRead}
              disabled={markAllAsReadMutation.isPending}
              className="h-7 text-xs"
            >
              <CheckCheck className="mr-1 h-3 w-3" />
              Mark all read
            </Button>
          )}
        </div>

        <ScrollArea className="h-[400px]">
          <div ref={viewportRef}>
            {isLoading ? (
              <div className="flex flex-col items-center justify-center py-8 text-center">
                <Loader2 className="mb-2 h-6 w-6 animate-spin text-muted-foreground" />
                <p className="text-sm text-muted-foreground">
                  Loading notifications...
                </p>
              </div>
            ) : !Array.isArray(notifications) || notifications.length === 0 ? (
              <div className="flex flex-col items-center justify-center py-8 text-center">
                <Bell className="mb-2 h-8 w-8 text-muted-foreground opacity-50" />
                <p className="text-sm text-muted-foreground">
                  No notifications
                </p>
              </div>
            ) : (
              <div className="divide-y">
                {notifications.map((notification: any) => (
                  <div
                    key={notification.id}
                    className={`p-4 transition-colors hover:bg-gray-50 ${
                      !notification.isRead ? 'bg-blue-50/50' : ''
                    }`}
                  >
                    <div className="flex items-start justify-between gap-2">
                      <div className="min-w-0 flex-1">
                        <div className="flex items-start gap-2">
                          {!notification.isRead && (
                            <div className="mt-2 h-2 w-2 flex-shrink-0 rounded-full bg-blue-600" />
                          )}
                          <div className="min-w-0 flex-1">
                            <p className="text-sm font-medium">
                              {notification.title}
                            </p>
                            {notification.message && (
                              <p className="mt-1 line-clamp-2 text-xs text-muted-foreground">
                                {notification.message}
                              </p>
                            )}
                            <p className="mt-1 text-xs text-muted-foreground">
                              {formatDistanceToNow(
                                new Date(notification.createdAt),
                                {
                                  addSuffix: true
                                }
                              )}
                            </p>
                          </div>
                        </div>
                      </div>
                      <div className="flex items-center gap-1">
                        {!notification.isRead && (
                          <Button
                            variant="ghost"
                            size="sm"
                            className="h-6 w-6 p-0"
                            onClick={() => handleMarkAsRead(notification.id)}
                            disabled={markAsReadMutation.isPending}
                          >
                            <Check className="h-3 w-3" />
                          </Button>
                        )}
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 w-6 p-0 text-destructive hover:text-destructive"
                          onClick={() => handleDelete(notification.id)}
                          disabled={deleteNotificationMutation.isPending}
                        >
                          <Trash2 className="h-3 w-3" />
                        </Button>
                      </div>
                    </div>
                  </div>
                ))}
                {isFetchingNextPage && (
                  <div className="flex items-center justify-center py-4">
                    <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
                    <span className="ml-2 text-xs text-muted-foreground">
                      Loading more...
                    </span>
                  </div>
                )}
              </div>
            )}
          </div>
        </ScrollArea>
      </PopoverContent>
    </Popover>
  );
}
