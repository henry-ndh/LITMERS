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
  useCreateIssue,
  useGetIssueStatusesByProjectId
} from '@/queries/issue.query';
import { useGetProjectDetail } from '@/queries/project.query';
import { useGetTeamMembers } from '@/queries/team.query';
import { useToast } from '@/components/ui/use-toast';

interface CreateIssueDialogProps {
  projectId: number;
  statusId?: number | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function CreateIssueDialog({
  projectId,
  statusId,
  open,
  onOpenChange
}: CreateIssueDialogProps) {
  const { toast } = useToast();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [selectedStatusId, setSelectedStatusId] = useState<string>('');
  const [priority, setPriority] = useState<string>('1'); // MEDIUM
  const [selectedAssigneeId, setSelectedAssigneeId] = useState<string>('');
  const [error, setError] = useState('');

  const { data: statusesData } = useGetIssueStatusesByProjectId(projectId);
  const { data: projectData } = useGetProjectDetail(projectId);
  const statuses = statusesData?.data || statusesData || [];
  const project = projectData?.data || projectData;
  const teamId = project?.teamId;

  const { data: membersData } = useGetTeamMembers(teamId);
  const members = membersData?.data || membersData || [];
  const createIssueMutation = useCreateIssue();

  useEffect(() => {
    if (open) {
      if (statusId) {
        setSelectedStatusId(statusId.toString());
      } else if (statuses.length > 0) {
        const defaultStatus =
          statuses.find((s: any) => s.isDefault) || statuses[0];
        setSelectedStatusId(defaultStatus.id.toString());
      }
    }
  }, [open, statusId, statuses]);

  const handleCreate = async () => {
    if (!title.trim()) {
      setError('Please enter a title');
      return;
    }

    if (!selectedStatusId) {
      setError('Please select a status');
      return;
    }

    setError('');

    try {
      const issuesInStatus =
        statuses.find((s: any) => s.id.toString() === selectedStatusId)
          ?.issueCount || 0;

      await createIssueMutation.mutateAsync({
        projectId,
        statusId: parseInt(selectedStatusId),
        title: title.trim(),
        description: description.trim() || undefined,
        priority: parseInt(priority),
        position: issuesInStatus,
        assigneeId: selectedAssigneeId
          ? parseInt(selectedAssigneeId)
          : undefined
      });
      toast({
        title: 'Success',
        description: 'Issue created successfully'
      });
      setTitle('');
      setDescription('');
      setPriority('1');
      setSelectedAssigneeId('');
      onOpenChange(false);
    } catch (err: any) {
      setError(err?.response?.data?.message || 'Failed to create issue');
      toast({
        title: 'Error',
        description: 'Failed to create issue',
        variant: 'destructive'
      });
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>Create New Issue</DialogTitle>
          <DialogDescription>
            Create a new issue to track work in your project
          </DialogDescription>
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
              disabled={createIssueMutation.isPending}
              maxLength={200}
            />
            <p className="text-xs text-muted-foreground">
              {title.length}/200 characters
            </p>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="status">Status *</Label>
              <Select
                value={selectedStatusId}
                onValueChange={setSelectedStatusId}
                disabled={createIssueMutation.isPending || !!statusId}
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
                disabled={createIssueMutation.isPending}
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
            <Label htmlFor="assignee">Assignee</Label>
            <Select
              value={selectedAssigneeId || 'unassigned'}
              onValueChange={(value) =>
                setSelectedAssigneeId(value === 'unassigned' ? '' : value)
              }
              disabled={createIssueMutation.isPending}
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
            <Label htmlFor="description">Description</Label>
            <Textarea
              id="description"
              placeholder="Issue description..."
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              disabled={createIssueMutation.isPending}
              rows={4}
              maxLength={1000}
            />
            <p className="text-xs text-muted-foreground">
              {description.length}/1000 characters
            </p>
          </div>
        </div>

        <DialogFooter>
          <Button
            variant="outline"
            onClick={() => {
              onOpenChange(false);
              setTitle('');
              setDescription('');
              setError('');
            }}
            disabled={createIssueMutation.isPending}
          >
            Cancel
          </Button>
          <Button
            onClick={handleCreate}
            disabled={createIssueMutation.isPending}
          >
            {createIssueMutation.isPending ? 'Creating...' : 'Create Issue'}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
