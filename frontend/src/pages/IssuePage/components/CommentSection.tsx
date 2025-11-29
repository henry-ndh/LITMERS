import { useState } from 'react';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import {
  useGetCommentsByIssueId,
  useCreateComment,
  useUpdateComment,
  useDeleteComment
} from '@/queries/comment.query';
import { useToast } from '@/components/ui/use-toast';
import { formatDistanceToNow } from 'date-fns';
import { MessageSquare, Edit, Trash2, Send } from 'lucide-react';
import __helpers from '@/helpers';

interface CommentSectionProps {
  issueId: number;
}

export default function CommentSection({ issueId }: CommentSectionProps) {
  const { toast } = useToast();
  const [newComment, setNewComment] = useState('');
  const [editingCommentId, setEditingCommentId] = useState<number | null>(null);
  const [editContent, setEditContent] = useState('');

  const { data: commentsData, isLoading } = useGetCommentsByIssueId(issueId);
  const createCommentMutation = useCreateComment();
  const updateCommentMutation = useUpdateComment();
  const deleteCommentMutation = useDeleteComment();

  const comments = commentsData?.data || commentsData || [];

  const getInitials = (name: string) => {
    return name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  };

  const handleCreateComment = async () => {
    if (!newComment.trim()) {
      toast({
        title: 'Error',
        description: 'Please enter a comment',
        variant: 'destructive'
      });
      return;
    }

    try {
      await createCommentMutation.mutateAsync({
        issueId,
        content: newComment.trim()
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Comment added successfully'
      });
      setNewComment('');
    } catch (error: any) {
      toast({
        title: 'Error',
        description: error?.response?.data?.message || 'Failed to add comment',
        variant: 'destructive'
      });
    }
  };

  const handleStartEdit = (comment: any) => {
    setEditingCommentId(comment.id);
    setEditContent(comment.content);
  };

  const handleCancelEdit = () => {
    setEditingCommentId(null);
    setEditContent('');
  };

  const handleUpdateComment = async (commentId: number) => {
    if (!editContent.trim()) {
      toast({
        title: 'Error',
        description: 'Please enter a comment',
        variant: 'destructive'
      });
      return;
    }

    try {
      await updateCommentMutation.mutateAsync({
        commentId,
        content: editContent.trim()
      });
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Comment updated successfully'
      });
      setEditingCommentId(null);
      setEditContent('');
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to update comment',
        variant: 'destructive'
      });
    }
  };

  const handleDeleteComment = async (commentId: number) => {
    if (!confirm('Are you sure you want to delete this comment?')) {
      return;
    }

    try {
      await deleteCommentMutation.mutateAsync(commentId);
      toast({
        title: 'Success',
        variant: 'success',
        description: 'Comment deleted successfully'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message || 'Failed to delete comment',
        variant: 'destructive'
      });
    }
  };

  return (
    <div className="space-y-4">
      {/* Comment Input */}
      <div className="space-y-2">
        <div className="flex items-start gap-3">
          <Avatar className="h-8 w-8">
            <AvatarFallback className="text-xs">U</AvatarFallback>
          </Avatar>
          <div className="flex-1 space-y-2">
            <Textarea
              placeholder="Add a comment..."
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
              disabled={createCommentMutation.isPending}
              rows={3}
              maxLength={1000}
              onKeyDown={(e) => {
                if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
                  handleCreateComment();
                }
              }}
            />
            <div className="flex items-center justify-between">
              <p className="text-xs text-muted-foreground">
                {newComment.length}/1000 characters
              </p>
              <Button
                size="sm"
                onClick={handleCreateComment}
                disabled={createCommentMutation.isPending || !newComment.trim()}
              >
                <Send className="mr-2 h-4 w-4" />
                {createCommentMutation.isPending ? 'Posting...' : 'Post'}
              </Button>
            </div>
          </div>
        </div>
      </div>

      {/* Comments List */}
      {isLoading ? (
        <div className="py-4 text-center text-sm text-muted-foreground">
          Loading comments...
        </div>
      ) : comments.length === 0 ? (
        <div className="py-8 text-center text-sm text-muted-foreground">
          <MessageSquare className="mx-auto mb-2 h-8 w-8 opacity-50" />
          <p>No comments yet</p>
        </div>
      ) : (
        <div className="space-y-4">
          {comments.map((comment: any) => (
            <div key={comment.id} className="flex items-start gap-3">
              <Avatar className="h-8 w-8">
                <AvatarFallback className="text-xs">
                  {getInitials(comment.userName || comment.userEmail || 'U')}
                </AvatarFallback>
              </Avatar>
              <div className="flex-1 space-y-1">
                <div className="flex items-center gap-2">
                  <span className="text-sm font-medium">
                    {comment.userName}
                  </span>
                  <span className="text-xs text-muted-foreground">
                    {formatDistanceToNow(new Date(comment.createdAt), {
                      addSuffix: true
                    })}
                  </span>
                  {comment.updatedAt !== comment.createdAt && (
                    <span className="text-xs text-muted-foreground">
                      (edited)
                    </span>
                  )}
                </div>
                {editingCommentId === comment.id ? (
                  <div className="space-y-2">
                    <Textarea
                      value={editContent}
                      onChange={(e) => setEditContent(e.target.value)}
                      disabled={updateCommentMutation.isPending}
                      rows={3}
                      maxLength={1000}
                    />
                    <div className="flex items-center gap-2">
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={handleCancelEdit}
                        disabled={updateCommentMutation.isPending}
                      >
                        Cancel
                      </Button>
                      <Button
                        size="sm"
                        onClick={() => handleUpdateComment(comment.id)}
                        disabled={
                          updateCommentMutation.isPending || !editContent.trim()
                        }
                      >
                        Save
                      </Button>
                    </div>
                  </div>
                ) : (
                  <p className="whitespace-pre-wrap text-sm">
                    {comment.content}
                  </p>
                )}
                {editingCommentId !== comment.id && comment.isOwner && (
                  <div className="flex items-center gap-2 pt-1">
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-6 text-xs"
                      onClick={() => handleStartEdit(comment)}
                    >
                      <Edit className="mr-1 h-3 w-3" />
                      Edit
                    </Button>
                    <Button
                      variant="ghost"
                      size="sm"
                      className="h-6 text-xs text-destructive hover:text-destructive"
                      onClick={() => handleDeleteComment(comment.id)}
                    >
                      <Trash2 className="mr-1 h-3 w-3" />
                      Delete
                    </Button>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
