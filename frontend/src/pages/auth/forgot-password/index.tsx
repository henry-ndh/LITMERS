'use client';

import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle
} from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useForgotPassword } from '@/queries/auth.query';
import { useToast } from '@/components/ui/use-toast';
import { Mail, ArrowLeft, CheckCircle2 } from 'lucide-react';
import Bg from '@/assets/bg.jpg';

export default function ForgotPasswordPage() {
  const { toast } = useToast();
  const [email, setEmail] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);

  const forgotPasswordMutation = useForgotPassword();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!email.trim()) {
      toast({
        title: 'Error',
        description: 'Please enter your email address',
        variant: 'destructive'
      });
      return;
    }

    try {
      await forgotPasswordMutation.mutateAsync({ email: email.trim() });
      setIsSuccess(true);
      toast({
        title: 'Success',
        variant: 'success',
        description:
          'If the email exists, a password reset link has been sent to your email.'
      });
    } catch (error: any) {
      toast({
        title: 'Error',
        description:
          error?.response?.data?.message ||
          error?.response?.data?.data ||
          'Failed to send reset email. Please try again.',
        variant: 'destructive'
      });
    }
  };

  return (
    <div className="relative h-screen flex-col items-center justify-center md:grid lg:max-w-none lg:grid-cols-2 lg:px-0">
      {/* Left column with background image */}
      <div className="relative hidden h-full flex-col bg-muted p-10 dark:border-r lg:flex">
        <div
          className="absolute inset-0 bg-secondary"
          style={{
            backgroundImage: `url(${Bg})`,
            backgroundSize: 'cover',
            backgroundPosition: 'center'
          }}
        />
        <div className="relative z-20 mt-auto">
          <blockquote className="space-y-2">
            <p className="text-lg text-white drop-shadow-lg">
              "The best way to get started is to quit talking and begin doing."
            </p>
            <footer className="text-sm text-white/80 drop-shadow">
              - Walt Disney
            </footer>
          </blockquote>
        </div>
      </div>

      {/* Right column with form */}
      <div className="flex h-full items-center justify-center p-4 lg:p-8">
        <Card className="w-full max-w-md">
          <CardHeader className="space-y-1">
            <CardTitle className="text-2xl font-bold">
              Forgot Password
            </CardTitle>
            <CardDescription>
              Enter your email address and we'll send you a link to reset your
              password.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {isSuccess ? (
              <div className="space-y-4">
                <Alert className="border-green-200 bg-green-50">
                  <CheckCircle2 className="h-4 w-4 text-green-600" />
                  <AlertDescription className="text-green-800">
                    If the email exists, a password reset link has been sent to
                    your email. Please check your inbox and follow the
                    instructions.
                  </AlertDescription>
                </Alert>
                <div className="space-y-2">
                  <Button
                    variant="outline"
                    className="w-full"
                    onClick={() => {
                      setIsSuccess(false);
                      setEmail('');
                    }}
                  >
                    Send Another Email
                  </Button>
                  <Button variant="ghost" className="w-full" asChild>
                    <Link to="/auth/signin">
                      <ArrowLeft className="mr-2 h-4 w-4" />
                      Back to Sign In
                    </Link>
                  </Button>
                </div>
              </div>
            ) : (
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="email">Email</Label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                    <Input
                      id="email"
                      type="email"
                      placeholder="name@example.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="pl-9"
                      disabled={forgotPasswordMutation.isPending}
                      required
                    />
                  </div>
                </div>

                <Button
                  type="submit"
                  className="w-full"
                  disabled={forgotPasswordMutation.isPending}
                >
                  {forgotPasswordMutation.isPending
                    ? 'Sending...'
                    : 'Send Reset Link'}
                </Button>

                <div className="text-center text-sm">
                  <Link
                    to="/auth/signin"
                    className="text-primary hover:underline inline-flex items-center gap-1"
                  >
                    <ArrowLeft className="h-3 w-3" />
                    Back to Sign In
                  </Link>
                </div>
              </form>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

