import { useParams, useNavigate } from 'react-router-dom';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle
} from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import {
  ArrowLeft,
  Calendar,
  User,
  Tag,
  CheckCircle2,
  Circle,
  Edit,
  Trash2
} from 'lucide-react';
import { useGetIssueDetail, useDeleteIssue } from '@/queries/issue.query';
import { IssuePriority, PRIORITY_CONFIG } from './components/IssueCard';
import { toast } from 'sonner';
import { format } from 'date-fns';

export default function IssueDetailPage() {
  const { projectId, issueId } = useParams<{
    projectId: string;
    issueId: string;
  }>();
  const navigate = useNavigate();

  const { data: issueData, isLoading } = useGetIssueDetail(issueId);
  const deleteIssueMutation = useDeleteIssue();

  const issue = issueData?.data || issueData;

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
      toast.success('Issue deleted successfully');
      navigate(`/project/${projectId}`);
    } catch (error: any) {
      toast.error(error?.response?.data?.message || 'Failed to delete issue');
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <div className="text-muted-foreground">Loading...</div>
      </div>
    );
  }

  if (!issue) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background">
        <div className="space-y-4 text-center">
          <div className="text-muted-foreground">Issue not found</div>
          <Button
            onClick={() => navigate(`/project/${projectId}`)}
            variant="outline"
          >
            Back to Project
          </Button>
        </div>
      </div>
    );
  }

  const priorityConfig =
    PRIORITY_CONFIG[issue.priority] || PRIORITY_CONFIG[IssuePriority.MEDIUM];
  const isOverdue = issue.dueDate && new Date(issue.dueDate) < new Date();

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  return (
    <div className="min-h-screen bg-background">
      {/* Navigation */}
      <div className="border-b">
        <div className="container mx-auto max-w-7xl px-4 py-3">
          <Button
            variant="ghost"
            onClick={() => navigate(`/project/${projectId}`)}
            className="gap-2"
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Project
          </Button>
        </div>
      </div>

      <div className="container mx-auto max-w-7xl px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <div className="flex items-start justify-between gap-4">
            <div className="flex-1">
              <div className="mb-4 flex items-center gap-3">
                <div
                  className="h-3 w-3 rounded-full"
                  style={{ backgroundColor: issue.statusColor || '#3b82f6' }}
                />
                <Badge variant="outline">{issue.statusName}</Badge>
                <Badge variant="outline" className={priorityConfig.color}>
                  {priorityConfig.label}
                </Badge>
              </div>
              <h1 className="mb-4 text-3xl font-bold tracking-tight">
                {issue.title}
              </h1>

              {issue.description && (
                <p className="mb-6 max-w-3xl text-muted-foreground">
                  {issue.description}
                </p>
              )}

              <div className="flex flex-wrap items-center gap-4">
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
                      <p className="text-sm font-medium">Assigned to</p>
                      <p className="text-xs text-muted-foreground">
                        {issue.assigneeName || issue.assigneeEmail}
                      </p>
                    </div>
                  </div>
                ) : (
                  <div className="flex items-center gap-2">
                    <div className="flex h-8 w-8 items-center justify-center rounded-full border-2 border-dashed border-muted-foreground">
                      <User className="h-4 w-4 text-muted-foreground" />
                    </div>
                    <div>
                      <p className="text-sm font-medium">Unassigned</p>
                    </div>
                  </div>
                )}

                {issue.dueDate && (
                  <div
                    className={`flex items-center gap-2 ${
                      isOverdue ? 'text-red-600' : 'text-muted-foreground'
                    }`}
                  >
                    <Calendar className="h-4 w-4" />
                    <div>
                      <p className="text-sm font-medium">Due Date</p>
                      <p className="text-xs">
                        {format(new Date(issue.dueDate), 'MMM d, yyyy')}
                      </p>
                    </div>
                  </div>
                )}
              </div>
            </div>

            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm">
                <Edit className="mr-2 h-4 w-4" />
                Edit
              </Button>
              <Button variant="destructive" size="sm" onClick={handleDelete}>
                <Trash2 className="mr-2 h-4 w-4" />
                Delete
              </Button>
            </div>
          </div>
        </div>

        {/* Tabs */}
        <Tabs defaultValue="details" className="space-y-6">
          <TabsList>
            <TabsTrigger value="details">Details</TabsTrigger>
            <TabsTrigger value="subtasks">
              Subtasks ({issue.subtasks?.length || 0})
            </TabsTrigger>
            <TabsTrigger value="labels">
              Labels ({issue.labels?.length || 0})
            </TabsTrigger>
            <TabsTrigger value="history">History</TabsTrigger>
          </TabsList>

          <TabsContent value="details" className="space-y-6">
            <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
              <Card>
                <CardHeader>
                  <CardTitle>Issue Information</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Project
                    </p>
                    <p className="mt-1">{issue.projectName}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Status
                    </p>
                    <div className="mt-1 flex items-center gap-2">
                      <div
                        className="h-3 w-3 rounded-full"
                        style={{
                          backgroundColor: issue.statusColor || '#3b82f6'
                        }}
                      />
                      <span>{issue.statusName}</span>
                    </div>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Owner
                    </p>
                    <p className="mt-1">{issue.ownerName}</p>
                  </div>
                  <div>
                    <p className="text-sm font-medium text-muted-foreground">
                      Created
                    </p>
                    <p className="mt-1">
                      {format(new Date(issue.createdAt), 'MMM d, yyyy HH:mm')}
                    </p>
                  </div>
                </CardContent>
              </Card>
            </div>
          </TabsContent>

          <TabsContent value="subtasks" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Subtasks</CardTitle>
                <CardDescription>
                  Manage subtasks for this issue (
                  {issue.completedSubtaskCount || 0}/{issue.subtaskCount || 0}{' '}
                  completed)
                </CardDescription>
              </CardHeader>
              <CardContent>
                {issue.subtasks && issue.subtasks.length > 0 ? (
                  <div className="space-y-2">
                    {issue.subtasks.map((subtask: any) => (
                      <div
                        key={subtask.id}
                        className="flex items-center gap-3 rounded-lg border p-3"
                      >
                        {subtask.isDone ? (
                          <CheckCircle2 className="h-5 w-5 text-green-600" />
                        ) : (
                          <Circle className="h-5 w-5 text-muted-foreground" />
                        )}
                        <span
                          className={
                            subtask.isDone
                              ? 'text-muted-foreground line-through'
                              : ''
                          }
                        >
                          {subtask.title}
                        </span>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-muted-foreground">No subtasks yet</p>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="labels" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>Labels</CardTitle>
                <CardDescription>Labels attached to this issue</CardDescription>
              </CardHeader>
              <CardContent>
                {issue.labels && issue.labels.length > 0 ? (
                  <div className="flex flex-wrap gap-2">
                    {issue.labels.map((label: any) => (
                      <Badge
                        key={label.id}
                        variant="outline"
                        style={{
                          backgroundColor: `${label.color}20`,
                          borderColor: label.color,
                          color: label.color
                        }}
                      >
                        <Tag className="mr-1 h-3 w-3" />
                        {label.name}
                      </Badge>
                    ))}
                  </div>
                ) : (
                  <p className="text-muted-foreground">No labels attached</p>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="history" className="space-y-6">
            <Card>
              <CardHeader>
                <CardTitle>History</CardTitle>
                <CardDescription>
                  Activity history for this issue
                </CardDescription>
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground">
                  History feature coming soon...
                </p>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
}
