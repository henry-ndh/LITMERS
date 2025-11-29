import { useState } from 'react';
import { Dialog, DialogContent } from '@/components/ui/dialog';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';

import {
  Calendar,
  User,
  CheckCircle2,
  Circle,
  Edit,
  Trash2,
  X,
  ChevronDown,
  ChevronUp,
  Clock,
  MessageSquare,
  Sparkles,
  Lightbulb
} from 'lucide-react';
import AssigneeSelect from './AssigneeSelect';
import CommentSection from './CommentSection';
import {
  useGetIssueDetail,
  useDeleteIssue,
  useUpdateSubtask,
  useDeleteSubtask,
  useCreateSubtask,
  useGetAISummary,
  useGetAISuggestion
} from '@/queries/issue.query';
import { useGetProjectDetail } from '@/queries/project.query';
import { useGetTeamMembers } from '@/queries/team.query';
import { IssuePriority, PRIORITY_CONFIG } from './IssueCard';
import { useToast } from '@/components/ui/use-toast';
import { format } from 'date-fns';

interface IssueDetailDialogProps {
  issueId?: number | null;
  projectId?: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onEdit?: (issue: any) => void;
}

export default function IssueDetailDialog({
  issueId,
  projectId,
  open,
  onOpenChange,
  onEdit
}: IssueDetailDialogProps) {
  const { toast } = useToast();
  const [isAddingSubtask, setIsAddingSubtask] = useState(false);
  const [newSubtaskTitle, setNewSubtaskTitle] = useState('');
  const [showAllSubtasks, setShowAllSubtasks] = useState(true);
  const [aiSummary, setAiSummary] = useState<string | null>(null);
  const [aiSuggestion, setAiSuggestion] = useState<string | null>(null);
  const [showAiSummary, setShowAiSummary] = useState(false);
  const [showAiSuggestion, setShowAiSuggestion] = useState(false);

  const { data: issueData, isLoading } = useGetIssueDetail(
    issueId || undefined
  );
  const { data: projectData } = useGetProjectDetail(projectId || undefined);
  const deleteIssueMutation = useDeleteIssue();
  const updateSubtaskMutation = useUpdateSubtask();
  const deleteSubtaskMutation = useDeleteSubtask();
  const createSubtaskMutation = useCreateSubtask();
  const getAISummaryMutation = useGetAISummary();
  const getAISuggestionMutation = useGetAISuggestion();

  const issue = issueData?.data || issueData;
  const project = projectData?.data || projectData;
  const teamId = project?.teamId;

  const { data: membersData } = useGetTeamMembers(teamId);
  const members = membersData?.data || membersData || [];

  const priorityConfig = issue
    ? PRIORITY_CONFIG[issue.priority] || PRIORITY_CONFIG[IssuePriority.MEDIUM]
    : null;
  const isOverdue = issue?.dueDate && new Date(issue.dueDate) < new Date();

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const handleDelete = async () => {
    if (!issue) return;

    if (
      !confirm(
        `Are you sure you want to delete "${issue.title}"? This action cannot be undone.`
      )
    ) {
      return;
    }

    try {
      await deleteIssueMutation.mutateAsync(issue.id);
      toast({
        title: 'Success',
        description: 'Issue deleted successfully'
      });
      onOpenChange(false);
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to delete issue',
        variant: 'destructive'
      });
    }
  };

  const handleCreateSubtask = async () => {
    if (!newSubtaskTitle.trim() || !issue) return;

    try {
      await createSubtaskMutation.mutateAsync({
        issueId: issue.id,
        title: newSubtaskTitle.trim(),
        position: issue.subtasks?.length || 0
      });
      toast({
        title: 'Success',
        description: 'Subtask created successfully'
      });
      setNewSubtaskTitle('');
      setIsAddingSubtask(false);
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to create subtask',
        variant: 'destructive'
      });
    }
  };

  if (!open || !issueId) return null;

  const completedSubtasks =
    issue?.subtasks?.filter((s: any) => s.isDone).length || 0;
  const totalSubtasks = issue?.subtasks?.length || 0;
  const progressPercentage =
    totalSubtasks > 0 ? (completedSubtasks / totalSubtasks) * 100 : 0;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex h-[90vh] max-w-6xl flex-col gap-0 p-0">
        {isLoading ? (
          <div className="flex items-center justify-center py-8">
            <div className="text-muted-foreground">Loading...</div>
          </div>
        ) : issue ? (
          <div className="flex h-full min-h-0 flex-col">
            {/* Header */}
            <div className="flex flex-shrink-0 items-center justify-between border-b bg-muted/30 px-6 py-4">
              <div className="flex items-center gap-3">
                <div
                  className="h-4 w-4 rounded"
                  style={{ backgroundColor: issue.statusColor || '#3b82f6' }}
                />
                <span className="text-sm font-medium text-muted-foreground">
                  BE-{issue.id}
                </span>
              </div>
              <div className="flex items-center gap-2">
                {onEdit && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => onEdit(issue)}
                  >
                    <Edit className="h-4 w-4" />
                  </Button>
                )}
                <Button variant="ghost" size="sm" onClick={handleDelete}>
                  <Trash2 className="h-4 w-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => onOpenChange(false)}
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            </div>

            {/* Content */}
            <div className="min-h-0 flex-1 overflow-y-auto">
              <div className="grid grid-cols-3 gap-6 p-6">
                {/* Main Content - Left 2/3 */}
                <div className="col-span-2 space-y-4">
                  {/* Title */}
                  <div className="rounded-lg border bg-background p-4">
                    <h1 className="text-2xl font-semibold">{issue.title}</h1>
                  </div>

                  {/* Description */}
                  <div className="rounded-lg border bg-background p-4">
                    <div className="mb-3 flex items-center justify-between">
                      <h3 className="text-sm font-semibold uppercase text-muted-foreground">
                        Description
                      </h3>
                      <div className="flex items-center gap-2">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={async () => {
                            if (!issueId) return;
                            try {
                              const [err, response] =
                                await getAISummaryMutation.mutateAsync(issueId);
                              if (err) {
                                toast({
                                  title: 'Error',
                                  description:
                                    err?.response?.data?.message ||
                                    'Failed to generate AI summary',
                                  variant: 'destructive'
                                });
                                return;
                              }
                              const data = (response as any)?.data || response;
                              const summary =
                                typeof data === 'string' ? data : data?.summary;
                              setAiSummary(summary);
                              setShowAiSummary(true);
                              toast({
                                title: 'Success',
                                description: (data as any)?.cached
                                  ? 'AI Summary loaded from cache'
                                  : 'AI Summary generated successfully',
                                variant: 'success'
                              });
                            } catch (error: any) {
                              toast({
                                title: 'Error',
                                description:
                                  error?.response?.data?.message ||
                                  error?.response?.data?.data ||
                                  'Failed to generate AI summary',
                                variant: 'destructive'
                              });
                            }
                          }}
                          disabled={getAISummaryMutation.isPending}
                          className="gap-2"
                        >
                          <Sparkles className="h-4 w-4" />
                          {getAISummaryMutation.isPending
                            ? 'Generating...'
                            : 'AI Summary'}
                        </Button>
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={async () => {
                            if (!issueId) return;
                            try {
                              const [err, response] =
                                await getAISuggestionMutation.mutateAsync(
                                  issueId
                                );
                              if (err) {
                                toast({
                                  title: 'Error',
                                  description:
                                    err?.response?.data?.message ||
                                    'Failed to generate AI suggestion',
                                  variant: 'destructive'
                                });
                                return;
                              }

                              console.log(response);
                              const data = (response as any)?.data || response;
                              const suggestion =
                                typeof data === 'string'
                                  ? data
                                  : data?.suggestion;
                              setAiSuggestion(suggestion);
                              setShowAiSuggestion(true);
                              toast({
                                title: 'Success',
                                description: (data as any)?.cached
                                  ? 'AI Suggestion loaded from cache'
                                  : 'AI Suggestion generated successfully',
                                variant: 'success'
                              });
                            } catch (error: any) {
                              toast({
                                title: 'Error',
                                description:
                                  error?.response?.data?.message ||
                                  error?.response?.data?.data ||
                                  'Failed to generate AI suggestion',
                                variant: 'destructive'
                              });
                            }
                          }}
                          disabled={getAISuggestionMutation.isPending}
                          className="gap-2"
                        >
                          <Lightbulb className="h-4 w-4" />
                          {getAISuggestionMutation.isPending
                            ? 'Generating...'
                            : 'AI Suggestion'}
                        </Button>
                      </div>
                    </div>
                    {issue.description ? (
                      <div className="whitespace-pre-wrap rounded-md bg-muted/30 p-4 text-sm text-foreground">
                        {issue.description}
                      </div>
                    ) : (
                      <div className="rounded-md bg-muted/30 p-4 text-sm italic text-muted-foreground">
                        Add a description...
                      </div>
                    )}
                  </div>

                  {/* AI Summary */}
                  {showAiSummary && aiSummary && (
                    <div className="rounded-lg border border-blue-200 bg-blue-50 p-4">
                      <div className="mb-3 flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <Sparkles className="h-5 w-5 text-blue-600" />
                          <h3 className="text-sm font-semibold text-blue-900">
                            AI Summary
                          </h3>
                        </div>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => {
                            setShowAiSummary(false);
                            setAiSummary(null);
                          }}
                          className="h-8 w-8 p-0"
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                      <div className="max-h-60 overflow-y-auto pr-2">
                        <div className="whitespace-pre-wrap text-sm leading-relaxed text-blue-900">
                          {aiSummary}
                        </div>
                      </div>
                    </div>
                  )}

                  {/* AI Suggestion */}
                  {showAiSuggestion && aiSuggestion && (
                    <div className="rounded-lg border border-amber-200 bg-amber-50 p-4">
                      <div className="mb-3 flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <Lightbulb className="h-5 w-5 text-amber-600" />
                          <h3 className="text-sm font-semibold text-amber-900">
                            AI Suggestion
                          </h3>
                        </div>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => {
                            setShowAiSuggestion(false);
                            setAiSuggestion(null);
                          }}
                          className="h-8 w-8 p-0"
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                      <div className="max-h-60 overflow-y-auto pr-2">
                        <div className="whitespace-pre-wrap text-sm leading-relaxed text-amber-900">
                          {aiSuggestion}
                        </div>
                      </div>
                    </div>
                  )}

                  {/* Subtasks */}
                  <div className="rounded-lg border bg-background p-4">
                    <div className="mb-3 flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <h3 className="text-sm font-semibold uppercase text-muted-foreground">
                          Subtasks
                        </h3>
                        {totalSubtasks > 0 && (
                          <span className="text-xs text-muted-foreground">
                            {completedSubtasks} of {totalSubtasks} done
                          </span>
                        )}
                      </div>
                      {totalSubtasks > 0 && (
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => setShowAllSubtasks(!showAllSubtasks)}
                          className="h-7 text-xs"
                        >
                          {showAllSubtasks ? (
                            <>
                              <ChevronUp className="mr-1 h-3 w-3" />
                              Hide
                            </>
                          ) : (
                            <>
                              <ChevronDown className="mr-1 h-3 w-3" />
                              Show
                            </>
                          )}
                        </Button>
                      )}
                    </div>

                    {/* Progress Bar */}
                    {totalSubtasks > 0 && (
                      <div className="mb-4">
                        <div className="h-2 w-full overflow-hidden rounded-full bg-muted">
                          <div
                            className="h-full bg-green-600 transition-all duration-300"
                            style={{ width: `${progressPercentage}%` }}
                          />
                        </div>
                      </div>
                    )}

                    {/* Subtasks List */}
                    {showAllSubtasks && (
                      <div className="space-y-2">
                        {issue.subtasks && issue.subtasks.length > 0 ? (
                          issue.subtasks.map((subtask: any) => (
                            <div
                              key={subtask.id}
                              className="flex items-start gap-3 rounded-lg border bg-muted/20 p-3"
                            >
                              {/* Checkbox */}
                              <button
                                onClick={async () => {
                                  try {
                                    await updateSubtaskMutation.mutateAsync({
                                      subtaskId: subtask.id,
                                      issueId: issue.id,
                                      isDone: !subtask.isDone
                                    });
                                  } catch (error: any) {
                                    toast({
                                      title: 'Error',
                                      description: 'Failed to update subtask',
                                      variant: 'destructive'
                                    });
                                  }
                                }}
                                className="mt-0.5 flex-shrink-0 transition-opacity hover:opacity-70"
                              >
                                {subtask.isDone ? (
                                  <CheckCircle2 className="h-5 w-5 text-green-600" />
                                ) : (
                                  <Circle className="h-5 w-5 text-muted-foreground" />
                                )}
                              </button>

                              {/* Content */}
                              <div className="min-w-0 flex-1">
                                <div className="mb-2 flex items-center gap-2">
                                  <span className="text-xs text-muted-foreground">
                                    BE-{subtask.id}
                                  </span>
                                  <Badge
                                    variant="outline"
                                    className="h-5 text-xs"
                                  >
                                    M
                                  </Badge>
                                </div>
                                <p
                                  className={`mb-3 text-sm ${
                                    subtask.isDone
                                      ? 'text-muted-foreground line-through'
                                      : 'text-foreground'
                                  }`}
                                >
                                  {subtask.title}
                                </p>

                                {/* Assignee Row */}
                                <div className="flex items-center gap-2">
                                  <span className="text-xs text-muted-foreground">
                                    Assignee:
                                  </span>
                                  {subtask.assigneeId ? (
                                    <div className="flex items-center gap-1.5">
                                      <Avatar className="h-6 w-6">
                                        <AvatarFallback className="text-xs">
                                          {getInitials(
                                            subtask.assigneeName ||
                                              subtask.assigneeEmail ||
                                              'U'
                                          )}
                                        </AvatarFallback>
                                      </Avatar>
                                      <span className="text-xs font-medium">
                                        {subtask.assigneeName}
                                      </span>
                                    </div>
                                  ) : (
                                    <span className="text-xs italic text-muted-foreground">
                                      Unassigned
                                    </span>
                                  )}
                                  <AssigneeSelect
                                    subtask={subtask}
                                    members={members}
                                    onAssign={async (assigneeId) => {
                                      try {
                                        await updateSubtaskMutation.mutateAsync(
                                          {
                                            subtaskId: subtask.id,
                                            issueId: issue.id,
                                            assigneeId: assigneeId ?? 0
                                          }
                                        );
                                        toast({
                                          title: 'Success',
                                          description:
                                            'Assignee updated successfully'
                                        });
                                      } catch (error: any) {
                                        toast({
                                          title: 'Error',
                                          description:
                                            'Failed to update assignee',
                                          variant: 'destructive'
                                        });
                                      }
                                    }}
                                  />
                                </div>
                              </div>

                              {/* Delete Button */}
                              <div className="flex flex-shrink-0 items-center">
                                <Button
                                  variant="ghost"
                                  size="sm"
                                  className="h-7 w-7 p-0 text-muted-foreground hover:text-destructive"
                                  onClick={async () => {
                                    if (
                                      !confirm(
                                        `Are you sure you want to delete "${subtask.title}"?`
                                      )
                                    ) {
                                      return;
                                    }
                                    try {
                                      await deleteSubtaskMutation.mutateAsync({
                                        subtaskId: subtask.id,
                                        issueId: issue.id
                                      });
                                      toast({
                                        title: 'Success',
                                        description:
                                          'Subtask deleted successfully'
                                      });
                                    } catch (error: any) {
                                      toast({
                                        title: 'Error',
                                        description: 'Failed to delete subtask',
                                        variant: 'destructive'
                                      });
                                    }
                                  }}
                                >
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
                            </div>
                          ))
                        ) : (
                          <div className="py-8 text-center text-sm text-muted-foreground">
                            No subtasks yet
                          </div>
                        )}

                        {/* Add Subtask Inline */}
                        {isAddingSubtask ? (
                          <div className="flex items-center gap-2 rounded-lg border bg-muted/20 p-3">
                            <Circle className="h-5 w-5 flex-shrink-0 text-muted-foreground" />
                            <Input
                              placeholder="What needs to be done?"
                              value={newSubtaskTitle}
                              onChange={(e) =>
                                setNewSubtaskTitle(e.target.value)
                              }
                              onKeyDown={(e) => {
                                if (e.key === 'Enter') {
                                  handleCreateSubtask();
                                } else if (e.key === 'Escape') {
                                  setIsAddingSubtask(false);
                                  setNewSubtaskTitle('');
                                }
                              }}
                              autoFocus
                              className="h-9 flex-1 border-0 bg-transparent text-sm focus-visible:ring-0"
                            />
                            <Button
                              size="sm"
                              className="h-8"
                              onClick={handleCreateSubtask}
                              disabled={
                                !newSubtaskTitle.trim() ||
                                createSubtaskMutation.isPending
                              }
                            >
                              Add
                            </Button>
                            <Button
                              variant="ghost"
                              size="sm"
                              className="h-8 w-8 p-0"
                              onClick={() => {
                                setIsAddingSubtask(false);
                                setNewSubtaskTitle('');
                              }}
                            >
                              <X className="h-4 w-4" />
                            </Button>
                          </div>
                        ) : (
                          <Button
                            variant="outline"
                            size="sm"
                            className="w-full justify-start border-dashed"
                            onClick={() => setIsAddingSubtask(true)}
                          >
                            + Create subtask
                          </Button>
                        )}
                      </div>
                    )}
                  </div>

                  {/* Comments */}
                  <div className="rounded-lg border bg-background p-4">
                    <div className="mb-4 flex items-center gap-2">
                      <MessageSquare className="h-4 w-4 text-muted-foreground" />
                      <h3 className="text-sm font-semibold uppercase text-muted-foreground">
                        Activity
                      </h3>
                      <span className="text-xs text-muted-foreground">
                        {issue.commentCount || 0} comments
                      </span>
                    </div>
                    <CommentSection issueId={issue.id} />
                  </div>
                </div>

                {/* Sidebar - Right 1/3 */}
                <div className="space-y-4">
                  {/* Status */}
                  <div className="rounded-lg border bg-background p-4">
                    <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                      Status
                    </label>
                    <Badge variant="outline" className="font-normal">
                      {issue.statusName}
                    </Badge>
                  </div>

                  {/* Priority */}
                  {priorityConfig && (
                    <div className="rounded-lg border bg-background p-4">
                      <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                        Priority
                      </label>
                      <Badge variant="outline" className={priorityConfig.color}>
                        {priorityConfig.label}
                      </Badge>
                    </div>
                  )}

                  {/* Assignee */}
                  <div className="rounded-lg border bg-background p-4">
                    <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                      Assignee
                    </label>
                    {issue.assigneeId ? (
                      <div className="flex items-center gap-2">
                        <Avatar className="h-8 w-8">
                          <AvatarFallback className="text-xs">
                            {getInitials(
                              issue.assigneeName || issue.assigneeEmail || 'U'
                            )}
                          </AvatarFallback>
                        </Avatar>
                        <div>
                          <p className="text-sm font-medium">
                            {issue.assigneeName}
                          </p>
                          <p className="text-xs text-muted-foreground">
                            {issue.assigneeEmail}
                          </p>
                        </div>
                      </div>
                    ) : (
                      <div className="flex items-center gap-2 text-muted-foreground">
                        <User className="h-4 w-4" />
                        <span className="text-sm">Unassigned</span>
                      </div>
                    )}
                  </div>

                  {/* Reporter */}
                  <div className="rounded-lg border bg-background p-4">
                    <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                      Reporter
                    </label>
                    <div className="flex items-center gap-2">
                      <Avatar className="h-8 w-8">
                        <AvatarFallback className="text-xs">
                          {getInitials(issue.ownerName || 'U')}
                        </AvatarFallback>
                      </Avatar>
                      <p className="text-sm font-medium">{issue.ownerName}</p>
                    </div>
                  </div>

                  {/* Labels */}
                  <div className="rounded-lg border bg-background p-4">
                    <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                      Labels
                    </label>
                    {issue.labels && issue.labels.length > 0 ? (
                      <div className="flex flex-wrap gap-1">
                        {issue.labels.map((label: any) => (
                          <Badge
                            key={label.id}
                            variant="outline"
                            className="text-xs"
                            style={{
                              backgroundColor: `${label.color}20`,
                              borderColor: label.color,
                              color: label.color
                            }}
                          >
                            {label.name}
                          </Badge>
                        ))}
                      </div>
                    ) : (
                      <span className="text-sm text-muted-foreground">
                        None
                      </span>
                    )}
                  </div>

                  {/* Due Date */}
                  {issue.dueDate && (
                    <div className="rounded-lg border bg-background p-4">
                      <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                        Due Date
                      </label>
                      <div
                        className={`flex items-center gap-2 ${
                          isOverdue ? 'text-red-600' : 'text-foreground'
                        }`}
                      >
                        <Calendar className="h-4 w-4" />
                        <span className="text-sm">
                          {format(new Date(issue.dueDate), 'MMM d, yyyy')}
                        </span>
                      </div>
                    </div>
                  )}

                  {/* Created */}
                  <div className="rounded-lg border bg-background p-4">
                    <label className="mb-3 block text-xs font-semibold uppercase text-muted-foreground">
                      Created
                    </label>
                    <div className="flex items-center gap-2 text-foreground">
                      <Clock className="h-4 w-4" />
                      <span className="text-sm">
                        {format(new Date(issue.createdAt), 'MMM d, yyyy HH:mm')}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        ) : (
          <div className="flex items-center justify-center py-8">
            <div className="text-muted-foreground">Issue not found</div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
