import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/components/ui/dialog';
import {
  useGetIssueStatusesByProjectId,
  useCreateIssueStatus,
  useUpdateIssueStatus,
  useDeleteIssueStatus
} from '@/queries/issue.query';
import { useToast } from '@/components/ui/use-toast';
import { Plus, Edit, Trash2, GripVertical } from 'lucide-react';
import { Badge } from '@/components/ui/badge';

interface IssueStatusManagementProps {
  projectId: number;
}

export default function IssueStatusManagement({
  projectId
}: IssueStatusManagementProps) {
  const { toast } = useToast();
  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [editingStatus, setEditingStatus] = useState<any>(null);
  const [name, setName] = useState('');
  const [color, setColor] = useState('#3B82F6');
  const [wipLimit, setWipLimit] = useState<number | undefined>(undefined);

  const { data: statusesData, isLoading } =
    useGetIssueStatusesByProjectId(projectId);
  const createStatusMutation = useCreateIssueStatus();
  const updateStatusMutation = useUpdateIssueStatus();
  const deleteStatusMutation = useDeleteIssueStatus();

  const statuses = statusesData?.data || statusesData || [];

  const handleCreate = async () => {
    if (!name.trim()) {
      toast({
        title: 'Error',
        description: 'Status name is required',
        variant: 'destructive'
      });
      return;
    }

    try {
      // Calculate position (add to end)
      const position = statuses.length;
      await createStatusMutation.mutateAsync({
        projectId,
        name: name.trim(),
        color,
        position,
        wipLimit: wipLimit || undefined
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Status created successfully'
      });
      setIsCreateOpen(false);
      setName('');
      setColor('#3B82F6');
      setWipLimit(undefined);
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to create status',
        variant: 'destructive'
      });
    }
  };

  const handleStartEdit = (status: any) => {
    setEditingStatus(status);
    setName(status.name);
    setColor(status.color || '#3B82F6');
    setWipLimit(status.wipLimit);
  };

  const handleUpdate = async () => {
    if (!name.trim()) {
      toast({
        title: 'Error',
        description: 'Status name is required',
        variant: 'destructive'
      });
      return;
    }

    try {
      await updateStatusMutation.mutateAsync({
        statusId: editingStatus.id,
        projectId,
        name: name.trim(),
        color,
        wipLimit: wipLimit || undefined
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Status updated successfully'
      });
      setEditingStatus(null);
      setName('');
      setColor('#3B82F6');
      setWipLimit(undefined);
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to update status',
        variant: 'destructive'
      });
    }
  };

  const handleDelete = async (statusId: number) => {
    if (
      !confirm(
        'Are you sure you want to delete this status? All issues in this status will be moved to the default status.'
      )
    ) {
      return;
    }

    try {
      await deleteStatusMutation.mutateAsync({
        statusId,
        projectId
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

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="text-lg font-semibold">Issue Statuses</h3>
        <Dialog open={isCreateOpen} onOpenChange={setIsCreateOpen}>
          <DialogTrigger asChild>
            <Button size="sm">
              <Plus className="mr-2 h-4 w-4" />
              Create Status
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create Status</DialogTitle>
              <DialogDescription>
                Create a new status column for the Kanban board
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="status-name">Name</Label>
                <Input
                  id="status-name"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Status name"
                  maxLength={50}
                />
              </div>
              <div className="space-y-2">
                <Label htmlFor="status-color">Color</Label>
                <div className="flex items-center gap-2">
                  <Input
                    id="status-color"
                    type="color"
                    value={color}
                    onChange={(e) => setColor(e.target.value)}
                    className="h-10 w-20"
                  />
                  <Input
                    value={color}
                    onChange={(e) => setColor(e.target.value)}
                    placeholder="#3B82F6"
                    maxLength={7}
                  />
                </div>
              </div>
              <div className="space-y-2">
                <Label htmlFor="wip-limit">WIP Limit (optional)</Label>
                <Input
                  id="wip-limit"
                  type="number"
                  value={wipLimit || ''}
                  onChange={(e) =>
                    setWipLimit(
                      e.target.value ? parseInt(e.target.value) : undefined
                    )
                  }
                  placeholder="No limit"
                  min={1}
                />
              </div>
            </div>
            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => {
                  setIsCreateOpen(false);
                  setName('');
                  setColor('#3B82F6');
                  setWipLimit(undefined);
                }}
              >
                Cancel
              </Button>
              <Button
                onClick={handleCreate}
                disabled={createStatusMutation.isPending}
              >
                Create
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {isLoading ? (
        <p className="text-sm text-muted-foreground">Loading statuses...</p>
      ) : statuses.length === 0 ? (
        <p className="text-sm text-muted-foreground">No statuses yet</p>
      ) : (
        <div className="space-y-2">
          {statuses.map((status: any) => (
            <div
              key={status.id}
              className="flex items-center justify-between rounded-lg border p-3"
            >
              <div className="flex items-center gap-3">
                <GripVertical className="h-4 w-4 text-muted-foreground" />
                <Badge
                  variant="outline"
                  style={{
                    backgroundColor: `${status.color || '#3B82F6'}20`,
                    borderColor: status.color || '#3B82F6',
                    color: status.color || '#3B82F6'
                  }}
                >
                  {status.name}
                </Badge>
                {status.isDefault && (
                  <Badge variant="secondary" className="text-xs">
                    Default
                  </Badge>
                )}
                {status.wipLimit && (
                  <span className="text-xs text-muted-foreground">
                    WIP: {status.wipLimit}
                  </span>
                )}
              </div>
              <div className="flex items-center gap-2">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={() => handleStartEdit(status)}
                >
                  <Edit className="h-4 w-4" />
                </Button>
                {!status.isDefault && (
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => handleDelete(status.id)}
                    className="text-destructive hover:text-destructive"
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Edit Dialog */}
      <Dialog
        open={!!editingStatus}
        onOpenChange={(open) => !open && setEditingStatus(null)}
      >
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Edit Status</DialogTitle>
            <DialogDescription>Update status information</DialogDescription>
          </DialogHeader>
          <div className="space-y-4 py-4">
            <div className="space-y-2">
              <Label htmlFor="edit-status-name">Name</Label>
              <Input
                id="edit-status-name"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Status name"
                maxLength={50}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-status-color">Color</Label>
              <div className="flex items-center gap-2">
                <Input
                  id="edit-status-color"
                  type="color"
                  value={color}
                  onChange={(e) => setColor(e.target.value)}
                  className="h-10 w-20"
                />
                <Input
                  value={color}
                  onChange={(e) => setColor(e.target.value)}
                  placeholder="#3B82F6"
                  maxLength={7}
                />
              </div>
            </div>
            <div className="space-y-2">
              <Label htmlFor="edit-wip-limit">WIP Limit (optional)</Label>
              <Input
                id="edit-wip-limit"
                type="number"
                value={wipLimit || ''}
                onChange={(e) =>
                  setWipLimit(
                    e.target.value ? parseInt(e.target.value) : undefined
                  )
                }
                placeholder="No limit"
                min={1}
              />
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setEditingStatus(null)}>
              Cancel
            </Button>
            <Button
              onClick={handleUpdate}
              disabled={updateStatusMutation.isPending}
            >
              Update
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
