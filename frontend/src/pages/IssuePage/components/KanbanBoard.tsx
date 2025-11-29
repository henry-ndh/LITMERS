import { useState, useEffect } from 'react';
import { ScrollArea } from '@/components/ui/scroll-area';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Plus,
  MoreVertical,
  Settings,
  Loader2,
  GripVertical,
  AlertCircle
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger
} from '@/components/ui/dropdown-menu';
import IssueCard from './IssueCard';
import CreateIssueDialog from './CreateIssueDialog';
import CreateStatusDialog from './CreateStatusDialog';
import IssueDetailDialog from './IssueDetailDialog';
import UpdateIssueDialog from './UpdateIssueDialog';
import {
  useGetIssueStatusesByProjectId,
  useGetIssuesByProjectId,
  useDeleteIssueStatus,
  useMoveIssue
} from '@/queries/issue.query';
import { useToast } from '@/components/ui/use-toast';
import { cn } from '@/lib/utils';

interface KanbanBoardProps {
  projectId: string | number;
  canManage?: boolean;
}

export default function KanbanBoard({
  projectId,
  canManage = false
}: KanbanBoardProps) {
  const { toast } = useToast();
  const [isCreateIssueOpen, setIsCreateIssueOpen] = useState(false);
  const [isCreateStatusOpen, setIsCreateStatusOpen] = useState(false);
  const [selectedStatusId, setSelectedStatusId] = useState<number | null>(null);
  const [selectedIssueId, setSelectedIssueId] = useState<number | null>(null);
  const [editingIssue, setEditingIssue] = useState<any | null>(null);
  const [draggedIssue, setDraggedIssue] = useState<any | null>(null);
  const [draggedOverStatusId, setDraggedOverStatusId] = useState<number | null>(
    null
  );
  const [dropPosition, setDropPosition] = useState<number | null>(null);

  // Optimistic update state
  const [optimisticIssues, setOptimisticIssues] = useState<any[] | null>(null);

  const { data: statusesData, isLoading: isLoadingStatuses } =
    useGetIssueStatusesByProjectId(projectId);

  const {
    data: issuesData,
    isLoading: isLoadingIssues,
    isFetching: isFetchingIssues
  } = useGetIssuesByProjectId(projectId);

  const deleteStatusMutation = useDeleteIssueStatus();
  const moveIssueMutation = useMoveIssue();

  const statuses = statusesData?.data || statusesData || [];
  const rawIssues = issuesData?.data || issuesData || [];

  // Use optimistic issues if available, otherwise use raw issues
  const issues = optimisticIssues || rawIssues;

  // Khi mutation move đã xong và query issues cũng fetch xong
  // => clear optimisticIssues, dùng data mới từ server, tránh bị giật
  useEffect(() => {
    if (!moveIssueMutation.isPending && !isFetchingIssues) {
      setOptimisticIssues(null);
    }
  }, [moveIssueMutation.isPending, isFetchingIssues]);

  // Group issues by status
  const issuesByStatus = statuses.reduce(
    (acc: Record<number, typeof issues>, status: any) => {
      acc[status.id] = issues.filter(
        (issue: any) => issue.statusId === status.id
      );
      return acc;
    },
    {}
  );

  const handleCreateIssue = (statusId: number) => {
    setSelectedStatusId(statusId);
    setIsCreateIssueOpen(true);
  };

  const handleDeleteStatus = async (statusId: number) => {
    const statusIssues = issuesByStatus[statusId] || [];

    if (statusIssues.length > 0) {
      toast({
        title: 'Cannot delete status',
        description: 'Cannot delete status with issues. Move issues first.',
        variant: 'destructive'
      });
      return;
    }

    if (!confirm('Are you sure you want to delete this status?')) {
      return;
    }

    try {
      await deleteStatusMutation.mutateAsync({
        statusId,
        projectId: Number(projectId)
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Status deleted successfully'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to delete status',
        variant: 'destructive'
      });
    }
  };

  const handleDragStart = (e: React.DragEvent, issue: any) => {
    setDraggedIssue(issue);
    e.dataTransfer.effectAllowed = 'move';
  };

  const handleDragEnd = () => {
    setDraggedIssue(null);
    setDraggedOverStatusId(null);
    setDropPosition(null);
  };

  const handleDragOver = (e: React.DragEvent, statusId: number) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'move';
    setDraggedOverStatusId(statusId);

    // Calculate drop position based on mouse Y position
    const container = e.currentTarget as HTMLElement;
    const scrollArea = container.querySelector(
      '[data-radix-scroll-area-viewport]'
    );
    if (!scrollArea) return;

    const cards = Array.from(scrollArea.querySelectorAll('[data-issue-card]'));
    const rect = scrollArea.getBoundingClientRect();
    const mouseY = e.clientY - rect.top + scrollArea.scrollTop;

    let newPosition = 0;
    for (let i = 0; i < cards.length; i++) {
      const card = cards[i] as HTMLElement;
      const cardRect = card.getBoundingClientRect();
      const cardY = cardRect.top - rect.top + scrollArea.scrollTop;
      const cardMiddle = cardY + cardRect.height / 2;

      if (mouseY < cardMiddle) {
        break;
      }
      newPosition = i + 1;
    }

    setDropPosition(newPosition);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    // Only reset if we're leaving the status column entirely
    const relatedTarget = e.relatedTarget as HTMLElement;
    if (!relatedTarget || !e.currentTarget.contains(relatedTarget)) {
      setDraggedOverStatusId(null);
      setDropPosition(null);
    }
  };

  const handleDrop = async (e: React.DragEvent, targetStatusId: number) => {
    e.preventDefault();

    if (!draggedIssue || draggedIssue.statusId === targetStatusId) {
      setDraggedIssue(null);
      setDraggedOverStatusId(null);
      setDropPosition(null);
      return;
    }

    const targetStatusIssues = issuesByStatus[targetStatusId] || [];
    const finalPosition =
      dropPosition !== null ? dropPosition : targetStatusIssues.length;

    // Optimistic update
    const updatedIssues = rawIssues.map((issue: any) => {
      if (issue.id === draggedIssue.id) {
        // Update dragged issue
        return { ...issue, statusId: targetStatusId, position: finalPosition };
      } else if (
        issue.statusId === targetStatusId &&
        issue.position >= finalPosition
      ) {
        // Shift down issues in target status
        return { ...issue, position: issue.position + 1 };
      } else if (
        issue.statusId === draggedIssue.statusId &&
        issue.position > draggedIssue.position
      ) {
        // Shift up issues in source status
        return { ...issue, position: issue.position - 1 };
      }
      return issue;
    });

    setOptimisticIssues(updatedIssues);
    setDraggedIssue(null);
    setDraggedOverStatusId(null);
    setDropPosition(null);

    try {
      // Call API
      await moveIssueMutation.mutateAsync({
        issueId: draggedIssue.id,
        statusId: targetStatusId,
        position: finalPosition,
        projectId: Number(projectId)
      });
    } catch (error: any) {
      // Revert optimistic update on error
      setOptimisticIssues(null);

      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to move issue',
        variant: 'destructive'
      });
    }
  };

  if (isLoadingStatuses || isLoadingIssues) {
    return (
      <div className="flex h-96 items-center justify-center">
        <div className="flex flex-col items-center gap-3">
          <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
          <p className="text-sm text-slate-500">Loading board...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="h-full overflow-hidden bg-slate-50">
      {/* Kanban Board */}
      {statuses.length === 0 ? (
        <div className="flex h-full items-center justify-center">
          <div className="max-w-md space-y-4 text-center">
            <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-blue-100">
              <GripVertical className="h-8 w-8 text-blue-600" />
            </div>
            <div>
              <h3 className="mb-2 text-lg font-semibold text-slate-800">
                No status columns yet
              </h3>
              <p className="mb-4 text-sm text-slate-600">
                Create your first status column to start organizing your work
              </p>
            </div>
            {canManage && (
              <CreateStatusDialog
                projectId={Number(projectId)}
                open={isCreateStatusOpen}
                onOpenChange={setIsCreateStatusOpen}
              >
                <Button className="bg-blue-600 hover:bg-blue-700">
                  <Plus className="mr-2 h-4 w-4" />
                  Create First Status
                </Button>
              </CreateStatusDialog>
            )}
          </div>
        </div>
      ) : (
        <ScrollArea className="h-full">
          <div className="flex min-h-full gap-4 p-6">
            {statuses.map((status: any) => {
              const statusIssues = issuesByStatus[status.id] || [];
              const sortedIssues = [...statusIssues].sort(
                (a: any, b: any) => a.position - b.position
              );
              const isWipExceeded =
                status.wipLimit && statusIssues.length >= status.wipLimit;
              const isOverLimit =
                status.wipLimit && statusIssues.length > status.wipLimit;
              const isDraggedOver = draggedOverStatusId === status.id;

              return (
                <div
                  key={status.id}
                  className={cn(
                    'flex w-[320px] flex-shrink-0 flex-col rounded-lg border border-slate-200 bg-white transition-all',
                    isDraggedOver &&
                      'scale-[1.02] shadow-lg ring-2 ring-blue-500'
                  )}
                  onDragOver={(e) => handleDragOver(e, status.id)}
                  onDragLeave={handleDragLeave}
                  onDrop={(e) => handleDrop(e, status.id)}
                >
                  {/* Column Header */}
                  <div className="flex items-center justify-between border-b border-slate-200 bg-slate-50 px-4 py-3">
                    <div className="flex min-w-0 flex-1 items-center gap-2.5">
                      <div
                        className="h-2.5 w-2.5 flex-shrink-0 rounded-full"
                        style={{ backgroundColor: status.color || '#3b82f6' }}
                      />
                      <h3 className="truncate text-sm font-semibold text-slate-800">
                        {status.name}
                      </h3>
                      <Badge
                        variant="secondary"
                        className={cn(
                          'text-xs font-medium',
                          isOverLimit && 'bg-red-100 text-red-700'
                        )}
                      >
                        {statusIssues.length}
                        {status.wipLimit && ` / ${status.wipLimit}`}
                      </Badge>
                    </div>
                    {canManage && (
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button
                            variant="ghost"
                            size="sm"
                            className="h-7 w-7 p-0 text-slate-500 hover:bg-slate-100 hover:text-slate-900"
                          >
                            <MoreVertical className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem className="cursor-pointer">
                            <Settings className="mr-2 h-4 w-4" />
                            Edit Status
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem
                            onClick={() => handleDeleteStatus(status.id)}
                            className="cursor-pointer text-red-600 focus:text-red-600"
                          >
                            Delete Status
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    )}
                  </div>

                  {/* WIP Limit Warning */}
                  {isWipExceeded && (
                    <div className="mx-4 mt-3 flex items-center gap-2 rounded-md border border-red-200 bg-red-50 px-3 py-2">
                      <AlertCircle className="h-4 w-4 flex-shrink-0 text-red-600" />
                      <span className="text-xs font-medium text-red-700">
                        {isOverLimit ? 'WIP limit exceeded' : 'At WIP limit'}
                      </span>
                    </div>
                  )}

                  {/* Column Content */}
                  <ScrollArea className="flex-1 px-3 py-3">
                    <div
                      className="space-y-2.5"
                      data-radix-scroll-area-viewport
                    >
                      {sortedIssues.length > 0 ? (
                        <>
                          {sortedIssues.map((issue: any, index: number) => (
                            <div key={issue.id}>
                              {/* Drop indicator above */}
                              {isDraggedOver &&
                                dropPosition === index &&
                                draggedIssue?.id !== issue.id && (
                                  <div className="mb-2 h-0.5 rounded-full bg-blue-500 shadow-lg shadow-blue-500/50" />
                                )}

                              <div
                                data-issue-card
                                className={cn(
                                  'transition-opacity duration-200',
                                  draggedIssue?.id === issue.id && 'opacity-50'
                                )}
                              >
                                <IssueCard
                                  issue={issue}
                                  onClick={(issue) =>
                                    setSelectedIssueId(issue.id)
                                  }
                                  onDragStart={handleDragStart}
                                  onDragEnd={handleDragEnd}
                                  isDragging={draggedIssue?.id === issue.id}
                                />
                              </div>
                            </div>
                          ))}

                          {/* Drop indicator at the end */}
                          {isDraggedOver &&
                            dropPosition === sortedIssues.length && (
                              <div className="mt-2 h-0.5 rounded-full bg-blue-500 shadow-lg shadow-blue-500/50" />
                            )}
                        </>
                      ) : (
                        <>
                          {/* Drop indicator for empty column */}
                          {isDraggedOver && dropPosition === 0 && (
                            <div className="mb-2 h-0.5 rounded-full bg-blue-500 shadow-lg shadow-blue-500/50" />
                          )}

                          <div className="flex flex-col items-center justify-center py-12 text-center">
                            <div className="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-slate-100">
                              <GripVertical className="h-6 w-6 text-slate-400" />
                            </div>
                            <p className="mb-1 text-sm text-slate-500">
                              No issues yet
                            </p>
                            <p className="text-xs text-slate-400">
                              Drag issues here or create new ones
                            </p>
                          </div>
                        </>
                      )}
                    </div>
                  </ScrollArea>

                  {/* Add Issue Button */}
                  <div className="border-t border-slate-200 p-2">
                    <Button
                      variant="ghost"
                      className="h-9 w-full justify-start gap-2 text-slate-600 hover:bg-slate-100 hover:text-slate-900"
                      onClick={() => handleCreateIssue(status.id)}
                    >
                      <Plus className="h-4 w-4" />
                      <span className="text-sm">Create issue</span>
                    </Button>
                  </div>
                </div>
              );
            })}

            {/* Add Status Column */}
            {canManage && (
              <div className="flex min-h-[400px] w-[320px] flex-shrink-0 items-center justify-center rounded-lg border-2 border-dashed border-slate-300 bg-white transition-all hover:border-blue-400 hover:bg-blue-50/50">
                <CreateStatusDialog
                  projectId={Number(projectId)}
                  open={isCreateStatusOpen}
                  onOpenChange={setIsCreateStatusOpen}
                >
                  <Button
                    variant="ghost"
                    className="gap-2 text-slate-600 hover:bg-transparent hover:text-blue-700"
                  >
                    <Plus className="h-4 w-4" />
                    Add Status Column
                  </Button>
                </CreateStatusDialog>
              </div>
            )}
          </div>
        </ScrollArea>
      )}

      <CreateIssueDialog
        projectId={Number(projectId)}
        statusId={selectedStatusId}
        open={isCreateIssueOpen}
        onOpenChange={(open) => {
          setIsCreateIssueOpen(open);
          if (!open) setSelectedStatusId(null);
        }}
      />

      <IssueDetailDialog
        issueId={selectedIssueId}
        projectId={Number(projectId)}
        open={!!selectedIssueId}
        onOpenChange={(open) => {
          if (!open) setSelectedIssueId(null);
        }}
        onEdit={(issue) => {
          setEditingIssue(issue);
          setSelectedIssueId(null);
        }}
      />

      <UpdateIssueDialog
        issueId={editingIssue?.id}
        projectId={Number(projectId)}
        issue={editingIssue}
        open={!!editingIssue}
        onOpenChange={(open) => {
          if (!open) setEditingIssue(null);
        }}
      />
    </div>
  );
}
