import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue
} from '@/components/ui/select';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { AlertTriangle } from 'lucide-react';
import {
  useUpdateIssue,
  useGetIssueStatusesByProjectId,
  useGetProjectLabels
} from '@/queries/issue.query';
import { useGetProjectDetail } from '@/queries/project.query';
import { useGetTeamMembers } from '@/queries/team.query';
import { useToast } from '@/components/ui/use-toast';

interface UpdateIssueDialogProps {
  issueId?: number | null;
  projectId?: number;
  issue?: {
    id: number;
    title: string;
    description?: string | null;
    statusId: number;
    assigneeId?: number | null;
    dueDate?: string | null;
    priority: number;
    labels?: Array<{ id: number }>;
    labelIds?: number[];
  };
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function UpdateIssueDialog({
  issueId,
  projectId,
  issue,
  open,
  onOpenChange
}: UpdateIssueDialogProps) {
  const { toast } = useToast();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [selectedStatusId, setSelectedStatusId] = useState<string>('');
  const [priority, setPriority] = useState<string>('1');
  const [selectedAssigneeId, setSelectedAssigneeId] = useState<string>('');
  const [dueDate, setDueDate] = useState<string>('');
  const [selectedLabelIds, setSelectedLabelIds] = useState<number[]>([]);
  const [error, setError] = useState('');

  const { data: statusesData } = useGetIssueStatusesByProjectId(projectId);
  const { data: labelsData } = useGetProjectLabels(projectId);
  const { data: projectData } = useGetProjectDetail(projectId);
  const statuses = statusesData?.data || statusesData || [];
  const labels = labelsData?.data || labelsData || [];
  const project = projectData?.data || projectData;
  const teamId = project?.teamId;

  const { data: membersData } = useGetTeamMembers(teamId);
  const members = membersData?.data || membersData || [];
  const updateIssueMutation = useUpdateIssue();

  useEffect(() => {
    if (open && issue) {
      setTitle(issue.title || '');
      setDescription(issue.description || '');
      setSelectedStatusId(issue.statusId?.toString() || '');
      setPriority(issue.priority?.toString() || '1');
      setSelectedAssigneeId(issue.assigneeId?.toString() || '');
      setDueDate(
        issue.dueDate ? new Date(issue.dueDate).toISOString().split('T')[0] : ''
      );
      // Get labelIds from issue.labels array if available
      const labelIds =
        issue.labels?.map((l: any) => l.id) || issue.labelIds || [];
      setSelectedLabelIds(labelIds);
      setError('');
    }
  }, [open, issue]);

  const handleUpdate = async () => {
    if (!issueId) return;

    if (!title.trim()) {
      setError('Please enter a title');
      return;
    }

    setError('');

    try {
      await updateIssueMutation.mutateAsync({
        issueId,
        title: title.trim(),
        description: description.trim() || undefined,
        statusId: selectedStatusId ? parseInt(selectedStatusId) : undefined,
        priority: parseInt(priority),
        assigneeId: selectedAssigneeId
          ? parseInt(selectedAssigneeId)
          : undefined,
        dueDate: dueDate || undefined,
        labelIds: selectedLabelIds.length > 0 ? selectedLabelIds : undefined
      });
      toast({
        title: 'Success',
        description: 'Issue updated successfully'
      });
      onOpenChange(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to update issue');
      toast({
        title: 'Error',
        description: 'Failed to update issue',
        variant: 'destructive'
      });
    }
  };

  const handleLabelToggle = (labelId: number) => {
    setSelectedLabelIds((prev) =>
      prev.includes(labelId)
        ? prev.filter((id) => id !== labelId)
        : [...prev, labelId]
    );
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Update Issue</DialogTitle>
          <DialogDescription>Update issue information</DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-4">
          {error && (
            <Alert variant="destructive">
              <AlertTriangle className="h-4 w-4" />
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="space-y-2">
            <Label htmlFor="title">Title *</Label>
            <Input
              id="title"
              placeholder="Issue title..."
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              disabled={updateIssueMutation.isPending}
              maxLength={200}
            />
            <p className="text-xs text-muted-foreground">
              {title.length}/200 characters
            </p>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="status">Status</Label>
              <Select
                value={selectedStatusId}
                onValueChange={setSelectedStatusId}
                disabled={updateIssueMutation.isPending}
              >
                <SelectTrigger id="status">
                  <SelectValue placeholder="Select status" />
                </SelectTrigger>
                <SelectContent>
                  {statuses.map((status: any) => (
                    <SelectItem key={status.id} value={status.id.toString()}>
                      {status.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label htmlFor="priority">Priority</Label>
              <Select
                value={priority}
                onValueChange={setPriority}
                disabled={updateIssueMutation.isPending}
              >
                <SelectTrigger id="priority">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="0">High</SelectItem>
                  <SelectItem value="1">Medium</SelectItem>
                  <SelectItem value="2">Low</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Issue description..."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              disabled={updateIssueMutation.isPending}
              rows={4}
              maxLength={1000}
            />
            <p className="text-xs text-muted-foreground">
              {description.length}/1000 characters
            </p>
          </div>

          <div className="space-y-2">
            <Label htmlFor="assignee">Assignee</Label>
            <Select
              value={selectedAssigneeId || 'unassigned'}
              onValueChange={(value) =>
                setSelectedAssigneeId(value === 'unassigned' ? '' : value)
              }
              disabled={updateIssueMutation.isPending}
            >
              <SelectTrigger id="assignee">
                <SelectValue placeholder="Unassigned" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="unassigned">Unassigned</SelectItem>
                {members.map((member: any) => (
                  <SelectItem
                    key={member.userId}
                    value={member.userId.toString()}
                  >
                    {member.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="dueDate">Due Date</Label>
            <Input
              id="dueDate"
              type="date"
              value={dueDate}
              onChange={(e) => setDueDate(e.target.value)}
              disabled={updateIssueMutation.isPending}
            />
          </div>

          {labels.length > 0 && (
            <div className="space-y-2">
              <Label>Labels</Label>
              <div className="flex flex-wrap gap-2">
                {labels.map((label: any) => (
                  <Button
                    key={label.id}
                    type="button"
                    variant={
                      selectedLabelIds.includes(label.id)
                        ? 'default'
                        : 'outline'
                    }
                    size="sm"
                    onClick={() => handleLabelToggle(label.id)}
                    className="text-xs"
                    style={
                      selectedLabelIds.includes(label.id)
                        ? {
                            backgroundColor: label.color,
                            borderColor: label.color,
                            color: 'white'
                          }
                        : {
                            borderColor: label.color,
                            color: label.color
                          }
                    }
                  >
                    {label.name}
                  </Button>
                ))}
              </div>
            </div>
          )}
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => {
              onOpenChange(false);
              setError('');
            }}
            disabled={updateIssueMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            onClick={handleUpdate}
            disabled={updateIssueMutation.isPending}
          >
            {updateIssueMutation.isPending ? 'Updating...' : 'Update Issue'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
